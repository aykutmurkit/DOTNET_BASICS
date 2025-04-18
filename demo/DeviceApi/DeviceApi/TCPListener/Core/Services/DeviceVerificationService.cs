using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using DeviceApi.TCPListener.Core.Interfaces;
using DeviceApi.TCPListener.Models.Dto;
using Entities.Concrete;

namespace DeviceApi.TCPListener.Core.Services
{
    /// <summary>
    /// Cihaz doğrulama işlemleri için servis implementasyonu
    /// </summary>
    public class DeviceVerificationService : IDeviceVerificationService
    {
        private readonly ILogger<DeviceVerificationService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        // Onaylı cihazların IMEI'lerini tutan koleksiyon (önbellek)
        private readonly HashSet<string> _approvedDevices;

        // Onaysız cihazların IMEI'lerini tutan koleksiyon
        private readonly HashSet<string> _unapprovedDevices;

        // Onaysız cihazların bağlantı bilgilerini tutan sözlük
        private readonly ConcurrentDictionary<string, UnapprovedDeviceInfo> _unapprovedDeviceInfo;

        // Log sıklığını kontrol etmek için kullanılan sözlük
        private readonly ConcurrentDictionary<string, DateTime> _lastLogTimeByImei;

        // Kara liste (blacklist) için sözlük - IMEI ve ne zamana kadar bloke olduğunu tutar
        private readonly ConcurrentDictionary<string, DateTime> _blacklistedImeis = new ConcurrentDictionary<string, DateTime>();

        // Hız sınırlama için sözlük - IMEI ve son istek zamanlarını tutar
        private readonly ConcurrentDictionary<string, Queue<DateTime>> _rateLimitTracker = new ConcurrentDictionary<string, Queue<DateTime>>();

        // Hız sınırlama sabitleri
        private const int RATE_LIMIT_MAX_REQUESTS = 5;       // Belirli sürede izin verilen maksimum istek sayısı
        private const int RATE_LIMIT_WINDOW_SECONDS = 1;     // Hız sınırlama penceresi (saniye)
        private const int BLACKLIST_DURATION_SECONDS = 300;  // Kara liste süresi (5 dakika)

        // Veritabanı sorguları için sınırlama
        private const int DB_QUERY_THROTTLE_SECONDS = 30;    // Aynı IMEI için veritabanı sorguları arasındaki minimum süre
        private readonly ConcurrentDictionary<string, DateTime> _lastDbQueryTime = new ConcurrentDictionary<string, DateTime>();

        // Bağlantı sayısı eşik değerleri
        private const int CONNECTION_WARNING_THRESHOLD = 10;    // Bu sayıdan sonra uyarı logları yazılır
        private const int CONNECTION_CRITICAL_THRESHOLD = 50;   // Bu sayıdan sonra kritik loglar yazılır
        private const int LOG_THROTTLE_INTERVAL_SECONDS = 120;  // Log kısıtlama süresi (saniye olarak) - 2 dakikaya çıkarıldı
        private const int MAX_CONNECTIONS_PER_DEVICE = 2000;    // Bir cihaz için maksimum bağlantı sayısı
        private const int LOG_REJECT_INTERVAL = 5000;           // Bağlantı reddetme logları için aralık (her 5000'de bir)
        private const int REJECT_LOG_THROTTLE_SECONDS = 600;    // Reddedilen bağlantıların loglanması için 10 dakika ara

        /// <summary>
        /// DeviceVerificationService constructor'ı
        /// </summary>
        public DeviceVerificationService(
            ILogger<DeviceVerificationService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _approvedDevices = new HashSet<string>();
            _unapprovedDevices = new HashSet<string>();
            _unapprovedDeviceInfo = new ConcurrentDictionary<string, UnapprovedDeviceInfo>();
            _lastLogTimeByImei = new ConcurrentDictionary<string, DateTime>();
        }

        /// <summary>
        /// Belirli bir IMEI için log kaydı yazılması gerekip gerekmediğini kontrol eder
        /// </summary>
        private bool ShouldLogForImei(string imei, int connectionCount, bool isConnectionRejected = false)
        {
            // Bağlantı reddedildi ise, sadece belirli aralıklarla logla
            if (isConnectionRejected)
            {
                // Özel bir key kullan
                string rejectKey = $"{imei}_reject";

                // Sadece her LOG_REJECT_INTERVAL sayısında bir (örn: 5000, 10000, 15000...)
                if (connectionCount % LOG_REJECT_INTERVAL == 0)
                {
                    if (_lastLogTimeByImei.TryGetValue(rejectKey, out DateTime lastRejectLogTime))
                    {
                        var timeSinceLastLog = DateTime.Now - lastRejectLogTime;
                        if (timeSinceLastLog.TotalSeconds >= REJECT_LOG_THROTTLE_SECONDS)
                        {
                            _lastLogTimeByImei[rejectKey] = DateTime.Now;
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        _lastLogTimeByImei[rejectKey] = DateTime.Now;
                        return true;
                    }
                }
                return false;
            }

            // Her zaman log yazılacak durumlar
            if (connectionCount == 1 || // İlk bağlantı her zaman loglanır
                connectionCount == CONNECTION_WARNING_THRESHOLD || // Uyarı eşiği
                connectionCount == CONNECTION_CRITICAL_THRESHOLD || // Kritik eşik
                connectionCount % 500 == 0) // Her 500 bağlantıda bir
            {
                _lastLogTimeByImei[imei] = DateTime.Now;
                return true;
            }

            // Son log zamanını kontrol et
            if (_lastLogTimeByImei.TryGetValue(imei, out DateTime lastLogTime))
            {
                // Son logdan bu yana yeterli süre geçmiş mi?
                var timeSinceLastLog = DateTime.Now - lastLogTime;
                if (timeSinceLastLog.TotalSeconds >= LOG_THROTTLE_INTERVAL_SECONDS)
                {
                    _lastLogTimeByImei[imei] = DateTime.Now;
                    return true;
                }
            }
            else
            {
                // İlk defa karşılaşılan bir IMEI
                _lastLogTimeByImei[imei] = DateTime.Now;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Belirli bir IMEI için hız sınırının aşılıp aşılmadığını kontrol eder
        /// </summary>
        private bool IsRateLimitExceeded(string imei)
        {
            var now = DateTime.Now;

            // IMEI için istek zamanlarını içeren kuyruk oluştur veya mevcut olanı al
            var requestTimes = _rateLimitTracker.GetOrAdd(imei, _ => new Queue<DateTime>());

            lock (requestTimes)
            {
                // Zaman penceresinden eski istekleri kuyruktan çıkar
                while (requestTimes.Count > 0 && (now - requestTimes.Peek()).TotalSeconds > RATE_LIMIT_WINDOW_SECONDS)
                {
                    requestTimes.Dequeue();
                }

                // Hız sınırını kontrol et
                if (requestTimes.Count >= RATE_LIMIT_MAX_REQUESTS)
                {
                    // Sınır aşıldı
                    return true;
                }

                // Mevcut istek zamanını ekle
                requestTimes.Enqueue(now);
                return false;
            }
        }

        /// <summary>
        /// Cihazın IMEI numarası ile onaylı olup olmadığını kontrol eder
        /// </summary>
        /// <param name="imei">Kontrol edilecek IMEI numarası</param>
        /// <returns>Cihaz onaylı ise true, değilse false</returns>
        public bool VerifyDeviceByImei(string imei)
        {
            if (string.IsNullOrEmpty(imei))
            {
                _logger.LogWarning("IMEI değeri boş veya null");
                return false;
            }

            // Kara listede olup olmadığını kontrol et
            if (_blacklistedImeis.TryGetValue(imei, out DateTime blacklistExpiry))
            {
                if (DateTime.Now < blacklistExpiry)
                {
                    // Kara liste süresi dolmamış, sessizce reddet (log yazmadan)
                    return false;
                }
                else
                {
                    // Kara liste süresi dolmuş, listeden çıkar
                    _blacklistedImeis.TryRemove(imei, out _);
                }
            }

            // Hız sınırlamasını kontrol et
            if (IsRateLimitExceeded(imei))
            {
                // Hız sınırını aştı, kara listeye ekle
                var expiryTime = DateTime.Now.AddSeconds(BLACKLIST_DURATION_SECONDS);
                _blacklistedImeis.AddOrUpdate(imei, expiryTime, (_, __) => expiryTime);

                _logger.LogWarning(
                    "IMEI: {Imei} hız sınırını aştı ve {Duration} saniye boyunca bloke edildi",
                    imei, BLACKLIST_DURATION_SECONDS);

                return false;
            }

            // Eğer cihaz zaten onaylı listesinde ise
            if (_approvedDevices.Contains(imei))
            {
                return true; // Sessizce kabul et, gereksiz loglama yapma
            }

            // Eğer cihaz zaten onaysız listesinde ise
            if (_unapprovedDevices.Contains(imei))
            {
                var connectionTime = DateTime.Now;
                int connectionCount = 1;

                // Bağlantı bilgilerini güncelle
                if (_unapprovedDeviceInfo.TryGetValue(imei, out var deviceInfo))
                {
                    deviceInfo.LastConnectionTime = connectionTime;
                    deviceInfo.ConnectionCount++;
                    connectionCount = deviceInfo.ConnectionCount;

                    // Maksimum bağlantı sayısı kontrolü
                    if (connectionCount > MAX_CONNECTIONS_PER_DEVICE)
                    {
                        // Bağlantı reddi için log kısıtlaması uygula
                        if (ShouldLogForImei(imei, connectionCount, true))
                        {
                            _logger.LogWarning(
                                "BAĞLANTI REDDEDİLDİ - IMEI: {Imei} için maksimum bağlantı sayısı aşıldı: {Count}",
                                imei, connectionCount);
                        }
                        return false;
                    }

                    // Log kısıtlama kontrolü
                    if (ShouldLogForImei(imei, connectionCount))
                    {
                        if (connectionCount >= CONNECTION_CRITICAL_THRESHOLD)
                        {
                            _logger.LogError(
                                "KRİTİK: Onaysız cihaz çok sayıda bağlantı deniyor - IMEI: {Imei}, Bağlantı Sayısı: {Count}",
                                imei, connectionCount);
                        }
                        else if (connectionCount >= CONNECTION_WARNING_THRESHOLD)
                        {
                            _logger.LogWarning(
                                "UYARI: Onaysız cihaz tekrarlayan bağlantı denemeleri yapıyor - IMEI: {Imei}, Bağlantı Sayısı: {Count}",
                                imei, connectionCount);
                        }
                        else
                        {
                            _logger.LogInformation(
                                "Onaysız cihaz bağlantısı - IMEI: {Imei}, Bağlantı Sayısı: {Count}",
                                imei, connectionCount);
                        }
                    }
                }
                else
                {
                    _logger.LogError("Onaysız cihaz listesinde olan bir IMEI için bilgi bulunamadı: {Imei}", imei);
                }

                return false; // Onaysız cihazı reddet
            }

            // Yeni bir IMEI, veritabanında kontrol et
            bool isApproved = false;

            try
            {
                // Veritabanı sorgu kısıtlaması uygula
                if (_lastDbQueryTime.TryGetValue(imei, out DateTime lastQueryTime))
                {
                    var timeSinceLastQuery = DateTime.Now - lastQueryTime;
                    if (timeSinceLastQuery.TotalSeconds < DB_QUERY_THROTTLE_SECONDS)
                    {
                        // Çok sık sorgu yapılıyor, veritabanı sorgusu atla
                        _logger.LogDebug(
                            "Veritabanı sorgusu kısıtlandı - IMEI: {Imei}, Son sorgudan bu yana: {Seconds} saniye",
                            imei, timeSinceLastQuery.TotalSeconds);
                        return false;
                    }
                }

                // Veritabanı sorgu zamanını güncelle
                _lastDbQueryTime[imei] = DateTime.Now;

                // Veritabanında cihazı kontrol et
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                    var device = deviceRepository.GetByImei(imei);

                    if (device != null)
                    {
                        // Device sınıfında IsActive property'si yok, varsayılan olarak true kabul edelim
                        isApproved = true; // Cihaz veritabanında varsa, aktif kabul et
                        if (isApproved)
                        {
                            // Onaylı cihazlar listesine ekle
                            _approvedDevices.Add(imei);
                            _logger.LogInformation("Veritabanında onaylı cihaz bulundu ve kaydedildi: {Imei}", imei);
                        }
                        else
                        {
                            // Aktif olmayan cihazı reddet
                            _logger.LogWarning("Veritabanında bulunan cihaz aktif değil, reddedildi: {Imei}", imei);
                        }
                    }
                    else
                    {
                        // Veritabanında bulunmayan cihazı onaysız olarak kaydet
                        _unapprovedDevices.Add(imei);

                        // Onaysız cihazın ilk bağlantı bilgilerini kaydet
                        _unapprovedDeviceInfo[imei] = new UnapprovedDeviceInfo
                        {
                            FirstConnectionTime = DateTime.Now,
                            LastConnectionTime = DateTime.Now,
                            ConnectionCount = 1,
                            CommunicationType = null,
                            IsRecommendedForApproval = false,
                            IsSavedToDatabase = false
                        };

                        _logger.LogWarning("Onaysız cihaz bağlantısı - IMEI: {Imei} (Yeni)", imei);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz doğrulama sırasında hata oluştu: {Imei}", imei);
                return false; // Hata durumunda reddet
            }

            return isApproved;
        }

        /// <summary>
        /// Onaylı cihazların IMEI listesini döndürür
        /// </summary>
        public IEnumerable<string> GetApprovedDevices()
        {
            return _approvedDevices.ToList();
        }

        /// <summary>
        /// Onaylı cihazların detaylı bilgilerini döndürür
        /// </summary>
        public IEnumerable<ApprovedDeviceDto> GetApprovedDevicesWithDetails()
        {
            var approvedDevices = _approvedDevices.ToList();
            var deviceDtos = new List<ApprovedDeviceDto>();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();

                foreach (var imei in approvedDevices)
                {
                    try
                    {
                        var device = deviceRepository.GetByImei(imei);
                        if (device != null)
                        {
                            deviceDtos.Add(MapToApprovedDeviceDto(device));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Onaylı cihaz detayları alınırken hata: {Imei}", imei);
                    }
                }
            }

            return deviceDtos;
        }

        /// <summary>
        /// Onaysız cihazların IMEI listesini döndürür
        /// </summary>
        public IEnumerable<string> GetUnapprovedDevices()
        {
            return _unapprovedDevices.ToList();
        }

        /// <summary>
        /// Onaysız cihazların detaylı bilgilerini döndürür
        /// </summary>
        public IEnumerable<UnapprovedDeviceDto> GetUnapprovedDevicesWithDetails()
        {
            var unapprovedDeviceDtos = new List<UnapprovedDeviceDto>();

            foreach (var imei in _unapprovedDevices)
            {
                if (_unapprovedDeviceInfo.TryGetValue(imei, out var info))
                {
                    unapprovedDeviceDtos.Add(new UnapprovedDeviceDto
                    {
                        Imei = imei,
                        FirstConnectionIp = info.ConnectionIpAddress,
                        FirstConnectionTime = info.FirstConnectionTime,
                        LastConnectionTime = info.LastConnectionTime,
                        ConnectionCount = info.ConnectionCount,
                        CommunicationType = info.CommunicationType,
                        IsRecommendedForApproval = info.IsRecommendedForApproval,
                        IsSavedToDatabase = info.IsSavedToDatabase
                    });
                }
            }

            return unapprovedDeviceDtos.OrderByDescending(d => d.LastConnectionTime)
                                       .ThenByDescending(d => d.ConnectionCount);
        }

        /// <summary>
        /// Device nesnesini ApprovedDeviceDto'ya dönüştürür
        /// </summary>
        private ApprovedDeviceDto MapToApprovedDeviceDto(Device device)
        {
            return new ApprovedDeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                Imei = device.IMEI,
                DeviceType = "Bilinmiyor", // Device sınıfında DeviceSettings property'si yok
                Platform = device.Platform?.Id.ToString() ?? "Bilinmiyor", // Platform sınıfında Name property'si yok
                Station = device.Platform?.Station?.Name ?? "Bilinmiyor",
                AddedDate = DateTime.Now, // Device sınıfında CreatedDate property'si yok
                LastConnectionIp = null, // Bu bilgi veritabanında tutulmuyor
                LastConnectionDate = null, // Bu bilgi veritabanında tutulmuyor
                Status = "Aktif", // Device sınıfında IsActive property'si yok
                IsActive = true // Device sınıfında IsActive property'si yok, varsayılan olarak true
            };
        }

        /// <summary>
        /// Onaysız cihaz bilgilerini tutan iç sınıf
        /// </summary>
        internal class UnapprovedDeviceInfo
        {
            public string ConnectionIpAddress { get; set; }
            public DateTime FirstConnectionTime { get; set; }
            public DateTime LastConnectionTime { get; set; }
            public int ConnectionCount { get; set; }
            public int? CommunicationType { get; set; }
            public bool IsRecommendedForApproval { get; set; }
            public bool IsSavedToDatabase { get; set; }
        }
    }
} 