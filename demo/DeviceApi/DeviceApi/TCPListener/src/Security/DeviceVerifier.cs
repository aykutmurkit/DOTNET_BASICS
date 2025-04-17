using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DeviceApi.TCPListener.Configuration;
using DeviceApi.TCPListener.Models;
using DeviceApi.TCPListener.Security.Models;

namespace DeviceApi.TCPListener.Security
{
    /// <summary>
    /// Cihaz doğrulama servisi
    /// </summary>
    public class DeviceVerifier : IDeviceVerifier
    {
        private readonly ILogger<DeviceVerifier> _logger;
        private readonly TcpListenerSettings _settings;
        
        // Onaylı cihazlar listesi (IMEI -> Device bilgisi)
        private readonly ConcurrentDictionary<string, DeviceInfo> _approvedDevices = new();
        
        // Kara listede olan cihazlar (IMEI -> Zaman)
        private readonly ConcurrentDictionary<string, DateTime> _blacklistedDevices = new();
        
        // Hız sınırı aşan cihazlar (IMEI -> Zaman)
        private readonly ConcurrentDictionary<string, DateTime> _rateLimitedDevices = new();
        
        // Onaysız cihazlar listesi (IMEI -> Zaman)
        private readonly ConcurrentDictionary<string, DeviceInfo> _unapprovedDevices = new();
        
        /// <summary>
        /// Cihaz doğrulama servisi constructoru
        /// </summary>
        public DeviceVerifier(
            ILogger<DeviceVerifier> logger,
            IOptions<TcpListenerSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
            
            // Örnek onaylı cihazlar ekleyelim (test için)
            InitializeMockData();
        }
        
        /// <summary>
        /// Örnek test verisi oluştur
        /// </summary>
        private void InitializeMockData()
        {
            // Örnek onaylı cihaz ekle
            _approvedDevices["123456789012345"] = new DeviceInfo
            {
                Imei = "123456789012345",
                Model = "Test Device 1",
                LastConnectionTime = DateTime.Now.AddDays(-1),
                ConnectionCount = 10,
                ApprovalDate = DateTime.Now.AddMonths(-1),
                Status = DeviceStatus.Approved
            };
            
            _approvedDevices["234567890123456"] = new DeviceInfo
            {
                Imei = "234567890123456",
                Model = "Test Device 2",
                LastConnectionTime = DateTime.Now.AddHours(-2),
                ConnectionCount = 5,
                ApprovalDate = DateTime.Now.AddDays(-10),
                Status = DeviceStatus.Approved
            };
            
            // Örnek kara listeye alınmış cihaz
            _blacklistedDevices["345678901234567"] = DateTime.Now.AddMinutes(-30);
            
            // Örnek onaysız cihaz
            _unapprovedDevices["456789012345678"] = new DeviceInfo
            {
                Imei = "456789012345678",
                Model = "Unknown Device",
                LastConnectionTime = DateTime.Now.AddMinutes(-10),
                ConnectionCount = 1,
                Status = DeviceStatus.Unapproved
            };
        }

        /// <summary>
        /// IMEI numarasına göre cihazın onaylı olup olmadığını kontrol eder
        /// </summary>
        public bool VerifyDeviceByImei(string imei)
        {
            if (string.IsNullOrEmpty(imei))
            {
                _logger.LogWarning("Boş IMEI numarası ile doğrulama isteği");
                return false;
            }
            
            // IMEI formatını kontrol et
            if (!IsValidImeiFormat(imei))
            {
                _logger.LogWarning("Geçersiz IMEI formatı: {Imei}", imei);
                return false;
            }
            
            // Kara listedeki cihazları kontrol et ve süresi dolmuş olanları temizle
            CleanExpiredBlacklistedDevices();
            
            // Cihaz kara listede mi kontrol et
            if (_blacklistedDevices.ContainsKey(imei))
            {
                _logger.LogWarning("Kara listedeki cihaz bağlantı girişimi: {Imei}", imei);
                return false;
            }
            
            // Onaylı cihazlar listesinde mi kontrol et
            bool isApproved = _approvedDevices.ContainsKey(imei);
            
            // Onaylı değilse, onaysız cihazlar listesine ekle
            if (!isApproved)
            {
                RecordUnapprovedDevice(imei);
            }
            else
            {
                // Onaylı cihazın bağlantı sayısını ve son bağlantı zamanını güncelle
                if (_approvedDevices.TryGetValue(imei, out DeviceInfo deviceInfo))
                {
                    deviceInfo.ConnectionCount++;
                    deviceInfo.LastConnectionTime = DateTime.Now;
                }
            }
            
            return isApproved;
        }

        /// <summary>
        /// Onaylı cihazların IMEI listesini döndürür
        /// </summary>
        public IEnumerable<string> GetApprovedDevices()
        {
            return _approvedDevices.Keys.ToList();
        }

        /// <summary>
        /// Onaylı cihazların detaylı bilgilerini döndürür
        /// </summary>
        public IEnumerable<ApprovedDeviceInfo> GetApprovedDevicesWithDetails()
        {
            var result = new List<ApprovedDeviceInfo>();
            
            foreach (var device in _approvedDevices.Values)
            {
                result.Add(new ApprovedDeviceInfo
                {
                    Imei = device.Imei,
                    Name = device.Model,
                    LastConnectionDate = device.LastConnectionTime,
                    CreatedAt = device.ApprovalDate ?? DateTime.Now,
                    // Diğer alanları varsayılan değerlerle doldur
                    Id = 0,
                    IpAddress = string.Empty,
                    Port = 0,
                    PlatformId = 0,
                    PlatformName = string.Empty,
                    StationName = string.Empty,
                    ActiveMessageCount = 0
                });
            }
            
            return result;
        }

        /// <summary>
        /// Onaysız cihazların IMEI listesini döndürür
        /// </summary>
        public IEnumerable<string> GetUnapprovedDevices()
        {
            return _unapprovedDevices.Keys.ToList();
        }

        /// <summary>
        /// Onaysız cihazların detaylı bilgilerini döndürür
        /// </summary>
        public IEnumerable<UnapprovedDeviceInfo> GetUnapprovedDevicesWithDetails()
        {
            var result = new List<UnapprovedDeviceInfo>();
            
            foreach (var device in _unapprovedDevices.Values)
            {
                result.Add(new UnapprovedDeviceInfo
                {
                    Imei = device.Imei,
                    ConnectionIpAddress = string.Empty, // Bu bilgi şu an elimizde yok
                    FirstConnectionTime = device.LastConnectionTime, // İlk bağlantı zamanını tutmuyoruz, varsayılan olarak son bağlantıyı kullan
                    LastConnectionTime = device.LastConnectionTime,
                    ConnectionCount = device.ConnectionCount,
                    CommunicationType = null,
                    IsRecommendedForApproval = device.ConnectionCount > _settings.MaxUnapprovedConnectionAttempts / 2,
                    IsSavedToDatabase = false
                });
            }
            
            return result;
        }

        /// <summary>
        /// Kara listedeki cihaz sayısını döndürür
        /// </summary>
        public int GetBlacklistedDeviceCount()
        {
            CleanExpiredBlacklistedDevices();
            return _blacklistedDevices.Count;
        }

        /// <summary>
        /// Hız sınırı aşan cihaz sayısını döndürür
        /// </summary>
        public int GetRateLimitedDeviceCount()
        {
            CleanExpiredRateLimitedDevices();
            return _rateLimitedDevices.Count;
        }
        
        /// <summary>
        /// Onaysız cihazı kaydeder
        /// </summary>
        private void RecordUnapprovedDevice(string imei)
        {
            // Eğer daha önce kaydedilmemişse ekle
            _unapprovedDevices.TryAdd(imei, new DeviceInfo
            {
                Imei = imei,
                Model = "Unknown",
                LastConnectionTime = DateTime.Now,
                ConnectionCount = 1,
                Status = DeviceStatus.Unapproved
            });
            
            // Mevcut kaydı güncelle
            if (_unapprovedDevices.TryGetValue(imei, out DeviceInfo deviceInfo))
            {
                deviceInfo.ConnectionCount++;
                deviceInfo.LastConnectionTime = DateTime.Now;
                
                // Belirli bir sayıda bağlantı denemesi yapıldıysa kara listeye al
                if (deviceInfo.ConnectionCount > _settings.MaxUnapprovedConnectionAttempts)
                {
                    _blacklistedDevices[imei] = DateTime.Now;
                    _logger.LogWarning("Çok fazla onaysız bağlantı denemesi nedeniyle cihaz kara listeye alındı: {Imei}", imei);
                }
            }
            
            _logger.LogInformation("Onaysız cihaz bağlantı girişimi kaydedildi: {Imei}", imei);
        }
        
        /// <summary>
        /// Süresi dolmuş kara liste kayıtlarını temizler
        /// </summary>
        private void CleanExpiredBlacklistedDevices()
        {
            DateTime now = DateTime.Now;
            TimeSpan blacklistDuration = TimeSpan.FromMinutes(_settings.BlacklistDurationMinutes);
            
            foreach (var item in _blacklistedDevices)
            {
                if (now - item.Value > blacklistDuration)
                {
                    _blacklistedDevices.TryRemove(item.Key, out _);
                    _logger.LogInformation("Süresi dolan cihaz kara listeden çıkarıldı: {Imei}", item.Key);
                }
            }
        }
        
        /// <summary>
        /// Süresi dolmuş hız sınırı kayıtlarını temizler
        /// </summary>
        private void CleanExpiredRateLimitedDevices()
        {
            DateTime now = DateTime.Now;
            TimeSpan rateLimitDuration = TimeSpan.FromMinutes(1); // 1 dakika
            
            foreach (var item in _rateLimitedDevices)
            {
                if (now - item.Value > rateLimitDuration)
                {
                    _rateLimitedDevices.TryRemove(item.Key, out _);
                }
            }
        }
        
        /// <summary>
        /// IMEI numarası formatının geçerli olup olmadığını kontrol eder
        /// </summary>
        private bool IsValidImeiFormat(string imei)
        {
            // IMEI formatı: 15 veya 16 haneli rakam
            return Regex.IsMatch(imei, @"^\d{15,16}$");
        }
    }
} 
