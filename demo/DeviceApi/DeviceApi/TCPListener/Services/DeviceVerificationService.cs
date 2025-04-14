using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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
        private readonly ConcurrentDictionary<string, bool> _approvedDevices = new();
        
        // Onaysız cihazların IMEI'lerini tutan koleksiyon
        private readonly ConcurrentDictionary<string, bool> _unapprovedDevices = new();
        
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
        }
        
        /// <summary>
        /// Cihazın IMEI numarası ile onaylı olup olmadığını kontrol eder
        /// </summary>
        /// <param name="imei">Kontrol edilecek IMEI numarası</param>
        /// <returns>Cihaz onaylı ise true, değilse false</returns>
        public bool VerifyDeviceByImei(string imei)
        {
            // Önbellekte varsa sonucu döndür
            if (_approvedDevices.TryGetValue(imei, out bool _))
            {
                _logger.LogInformation("IMEI {Imei} onaylı cihazlar listesinde bulundu", imei);
                return true;
            }
            
            // Onaysız cihazlar listesinde varsa false döndür
            if (_unapprovedDevices.TryGetValue(imei, out bool _))
            {
                _logger.LogInformation("IMEI {Imei} onaysız cihazlar listesinde bulundu", imei);
                return false;
            }
            
            try
            {
                // Veritabanında kontrol et
                var isApproved = CheckDeviceInDatabase(imei);
                
                if (isApproved)
                {
                    // Onaylı cihazlar listesine ekle
                    _approvedDevices.TryAdd(imei, true);
                    _logger.LogInformation("IMEI {Imei} onaylı cihazlar listesine eklendi", imei);
                }
                else
                {
                    // Onaysız cihazlar listesine ekle
                    _unapprovedDevices.TryAdd(imei, true);
                    _logger.LogInformation("IMEI {Imei} onaysız cihazlar listesine eklendi", imei);
                }
                
                return isApproved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IMEI {Imei} doğrulaması sırasında hata oluştu", imei);
                
                // Hata durumunda güvenli tarafta kalarak false döndür
                _unapprovedDevices.TryAdd(imei, true);
                return false;
            }
        }
        
        /// <summary>
        /// Cihazın IMEI numarası ile veritabanında onaylı olup olmadığını kontrol eder
        /// </summary>
        private bool CheckDeviceInDatabase(string imei)
        {
            try
            {
                // Yeni bir scope oluştur
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    // Scope içinden IDeviceRepository'yi al
                    var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                    
                    // Asenkron metodu senkron çağırmak için .Result kullanıyoruz
                    // Not: Gerçek uygulamada, tüm zincir asenkron olmalıdır
                    var deviceExists = deviceRepository.ImeiExistsAsync(imei).Result;
                    
                    _logger.LogInformation("IMEI {Imei} veritabanında {Status}", 
                        imei, deviceExists ? "bulundu" : "bulunamadı");
                    
                    return deviceExists;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IMEI {Imei} veritabanı sorgusu sırasında hata oluştu", imei);
                throw;
            }
        }
        
        /// <summary>
        /// Onaylı cihazların listesini döndürür
        /// </summary>
        /// <returns>Onaylı cihazların IMEI listesi</returns>
        public IEnumerable<string> GetApprovedDevices()
        {
            return _approvedDevices.Keys.ToList();
        }
        
        /// <summary>
        /// Onaysız cihazların listesini döndürür
        /// </summary>
        /// <returns>Onaysız cihazların IMEI listesi</returns>
        public IEnumerable<string> GetUnapprovedDevices()
        {
            return _unapprovedDevices.Keys.ToList();
        }
    }
} 