using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using DeviceApi.TCPListener.Models.Devices;
using DeviceApi.TCPListener.Models.Protocol;
using Entities.Concrete;

namespace DeviceApi.TCPListener.Services
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
        /// <param name="logger">Loglama için gerekli logger nesnesi</param>
        /// <param name="serviceScopeFactory">Scope oluşturmak için factory</param>
        public DeviceVerificationService(
            ILogger<DeviceVerificationService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
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
                                "UYARI: Onaysız cihaz tekrar bağlandı - IMEI: {Imei}, Bağlantı Sayısı: {Count}, Son Bağlantı: {Time}",
                                imei, connectionCount, connectionTime);
                        }
                        else
                        {
                            _logger.LogInformation(
                                "Onaysız cihaz tekrar bağlandı - IMEI: {Imei}, Bağlantı Sayısı: {Count}",
                                imei, connectionCount);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning(
                        "Onaysız cihaz listede var fakat detay bilgisi yok - IMEI: {Imei}, Bağlantı Zamanı: {Time}",
                        imei, connectionTime);
                }
                
                return false;
            }

            try
            {
                // Veritabanı sorgusu için rate limiting uygula
                bool shouldQueryDb = true;
                
                if (_lastDbQueryTime.TryGetValue(imei, out DateTime lastQueryTime))
                {
                    var timeSinceLastQuery = DateTime.Now - lastQueryTime;
                    if (timeSinceLastQuery.TotalSeconds < DB_QUERY_THROTTLE_SECONDS)
                    {
                        // Son sorgulama üzerinden yeterli zaman geçmedi, veritabanına gitmeden reddet
                        shouldQueryDb = false;
                        
                        // Geçici olarak onaysız listesine ekle
                        if (!_unapprovedDevices.Contains(imei))
                        {
                            _unapprovedDevices.Add(imei);
                            
                            var connectionTime = DateTime.Now;
                            var deviceInfo = new UnapprovedDeviceInfo
                            {
                                FirstConnectionTime = connectionTime,
                                LastConnectionTime = connectionTime,
                                ConnectionCount = 1,
                                IsRecommendedForApproval = false,
                                IsSavedToDatabase = false
                            };
                            _unapprovedDeviceInfo.TryAdd(imei, deviceInfo);
                        }
                        
                        return false;
                    }
                }
                
                if (shouldQueryDb)
                {
                    // Veritabanı sorgu zamanını güncelle
                    _lastDbQueryTime[imei] = DateTime.Now;
                    
                    using var scope = _serviceScopeFactory.CreateScope();
                    var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                    
                    // IMEI'ye göre doğrudan veritabanında kontrol et
                    bool deviceExists = deviceRepository.ImeiExistsAsync(imei).GetAwaiter().GetResult();
                    
                    if (deviceExists)
                    {
                        _approvedDevices.Add(imei);
                        _logger.LogInformation($"Cihaz {imei} onaylandı");
                        return true;
                    }
                    else
                    {
                        _unapprovedDevices.Add(imei);
                        
                        var connectionTime = DateTime.Now;
                        
                        // Onaysız cihaz bilgilerini kaydet
                        var deviceInfo = new UnapprovedDeviceInfo
                        {
                            FirstConnectionTime = connectionTime,
                            LastConnectionTime = connectionTime,
                            ConnectionCount = 1,
                            IsRecommendedForApproval = false,
                            IsSavedToDatabase = false
                        };
                        
                        _unapprovedDeviceInfo.TryAdd(imei, deviceInfo);
                        
                        _logger.LogWarning(
                            "YENİ ONAYSIZ CİHAZ TESPİT EDİLDİ - IMEI: {Imei}, İlk Bağlantı Zamanı: {Time}",
                            imei, connectionTime);
                        
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Cihaz doğrulama sırasında hata oluştu - IMEI: {Imei}, Hata: {Error}",
                    imei, ex.Message);
                    
                _unapprovedDevices.Add(imei);
                return false;
            }
        }
        
        /// <summary>
        /// Onaylı cihazların listesini döndürür
        /// </summary>
        /// <returns>Onaylı cihazların IMEI listesi</returns>
        public IEnumerable<string> GetApprovedDevices()
        {
            return _approvedDevices.ToList();
        }
        
        /// <summary>
        /// Onaylı cihazların detaylı bilgilerini döndürür
        /// </summary>
        /// <returns>Onaylı cihazların detaylı bilgileri</returns>
        public IEnumerable<ApprovedDeviceDto> GetApprovedDevicesWithDetails()
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                
                var devices = new List<ApprovedDeviceDto>();
                var allDevices = deviceRepository.GetAllDevicesAsync().GetAwaiter().GetResult();
                
                foreach (var imei in _approvedDevices)
                {
                    try
                    {
                        var device = allDevices.FirstOrDefault(d => d.IMEI == imei);
                        if (device != null)
                        {
                            devices.Add(MapToApprovedDeviceDto(device));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"IMEI {imei} için cihaz detayları alınırken hata oluştu: {ex.Message}");
                    }
                }
                
                return devices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Onaylı cihazların detayları alınırken hata oluştu: {ex.Message}");
                return Enumerable.Empty<ApprovedDeviceDto>();
            }
        }
        
        /// <summary>
        /// Onaysız cihazların listesini döndürür
        /// </summary>
        /// <returns>Onaysız cihazların IMEI listesi</returns>
        public IEnumerable<string> GetUnapprovedDevices()
        {
            _logger.LogInformation("Onaysız cihaz listesi alınıyor - Cihaz Sayısı: {Count}", _unapprovedDevices.Count);
            return _unapprovedDevices.ToList();
        }
        
        /// <summary>
        /// Onaysız cihazların detaylı bilgilerini döndürür
        /// </summary>
        /// <returns>Onaysız cihazların detaylı bilgileri</returns>
        public IEnumerable<UnapprovedDeviceDto> GetUnapprovedDevicesWithDetails()
        {
            _logger.LogInformation("Onaysız cihazların detaylı bilgileri alınıyor - Cihaz Sayısı: {Count}", _unapprovedDevices.Count);
            
            var devices = new List<UnapprovedDeviceDto>();
            
            // Bağlantı sayısına göre sırala
            var sortedImeis = _unapprovedDevices
                .Where(imei => _unapprovedDeviceInfo.ContainsKey(imei))
                .OrderByDescending(imei => _unapprovedDeviceInfo[imei].ConnectionCount)
                .ToList();
            
            // Sıralanmamış IMEI'leri ekle
            sortedImeis.AddRange(_unapprovedDevices.Where(imei => !_unapprovedDeviceInfo.ContainsKey(imei)));
            
            foreach (var imei in sortedImeis)
            {
                try
                {
                    if (_unapprovedDeviceInfo.TryGetValue(imei, out var deviceInfo))
                    {
                        var dto = new UnapprovedDeviceDto
                        {
                            Imei = imei,
                            ConnectionIpAddress = deviceInfo.ConnectionIpAddress,
                            FirstConnectionTime = deviceInfo.FirstConnectionTime,
                            LastConnectionTime = deviceInfo.LastConnectionTime,
                            ConnectionCount = deviceInfo.ConnectionCount,
                            CommunicationType = deviceInfo.CommunicationType,
                            IsRecommendedForApproval = deviceInfo.IsRecommendedForApproval,
                            IsSavedToDatabase = deviceInfo.IsSavedToDatabase
                        };
                        
                        devices.Add(dto);
                        
                        // ConnectionCount'a göre log seviyesini belirle
                        if (deviceInfo.ConnectionCount >= CONNECTION_CRITICAL_THRESHOLD)
                        {
                            _logger.LogError(
                                "KRİTİK: Onaysız cihaz çok sayıda bağlantı deniyor - IMEI: {Imei}, Bağlantı Sayısı: {Count}",
                                imei, deviceInfo.ConnectionCount);
                        }
                        else if (deviceInfo.ConnectionCount >= CONNECTION_WARNING_THRESHOLD && 
                                 ShouldLogForImei(imei, deviceInfo.ConnectionCount))
                        {
                            _logger.LogDebug(
                                "Onaysız cihaz bilgisi - IMEI: {Imei}, İlk Bağlantı: {FirstConn}, Son Bağlantı: {LastConn}, Sayı: {Count}",
                                imei, deviceInfo.FirstConnectionTime, deviceInfo.LastConnectionTime, deviceInfo.ConnectionCount);
                        }
                    }
                    else
                    {
                        // IMEI var ama detay bilgisi yok
                        var now = DateTime.Now;
                        var dto = new UnapprovedDeviceDto
                        {
                            Imei = imei,
                            FirstConnectionTime = now,
                            LastConnectionTime = now,
                            ConnectionCount = 1,
                            IsRecommendedForApproval = false,
                            IsSavedToDatabase = false
                        };
                        
                        devices.Add(dto);
                        
                        _logger.LogWarning(
                            "Onaysız cihaz detay bilgisi eksik - IMEI: {Imei}, Varsayılan bağlantı zamanı: {Time}",
                            imei, now);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "IMEI {Imei} için onaysız cihaz detayları alınırken hata oluştu: {Error}",
                        imei, ex.Message);
                }
            }
            
            if (devices.Count > 0)
            {
                var criticalDevices = devices.Count(d => d.ConnectionCount >= CONNECTION_CRITICAL_THRESHOLD);
                var warningDevices = devices.Count(d => d.ConnectionCount >= CONNECTION_WARNING_THRESHOLD && d.ConnectionCount < CONNECTION_CRITICAL_THRESHOLD);
                
                _logger.LogInformation(
                    "Toplam {Total} adet onaysız cihaz bilgisi döndürülüyor (Kritik: {Critical}, Uyarı: {Warning})",
                    devices.Count, criticalDevices, warningDevices);
            }
            else
            {
                _logger.LogInformation("Onaysız cihaz bilgisi bulunamadı");
            }
            
            return devices;
        }
        
        /// <summary>
        /// Device entity'sini ApprovedDeviceDto'ya dönüştürür
        /// </summary>
        private ApprovedDeviceDto MapToApprovedDeviceDto(Device device)
        {
            // Platform entity'sinde Name özelliği yok - varsayılan değer kullanılıyor
            string platformName = "Platform " + device.PlatformId;
            
            return new ApprovedDeviceDto
            {
                Id = device.Id,
                Imei = device.IMEI,
                IpAddress = device.Ip,
                Port = device.Port,
                Name = device.Name,
                PlatformId = device.PlatformId,
                // Platform.Name property'si yok, istasyon adını veya varsayılan değer kullanıyoruz
                PlatformName = device.Platform?.Station != null ? device.Platform.Station.Name : platformName,
                StationName = device.Platform?.Station?.Name,
                LastConnectionDate = DateTime.Now,
                // Device entity'sinde CreatedAt/UpdatedAt özellikleri yok
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                ActiveMessageCount = 0
            };
        }
    }
    
    /// <summary>
    /// Onaysız cihazlar için ek bilgileri tutan yardımcı sınıf
    /// </summary>
    internal class UnapprovedDeviceInfo
    {
        public string ConnectionIpAddress { get; set; }
        public DateTime FirstConnectionTime { get; set; }
        public DateTime LastConnectionTime { get; set; }
        public int ConnectionCount { get; set; }
        public CommunicationType? CommunicationType { get; set; }
        public bool IsRecommendedForApproval { get; set; }
        public bool IsSavedToDatabase { get; set; }
    }
} 