using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using DeviceApi.TCPListener.Models;
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

            // Eğer cihaz zaten onaylı listesinde ise
            if (_approvedDevices.Contains(imei))
            {
                _logger.LogInformation($"Cihaz {imei} önceden onaylanmış");
                return true;
            }

            // Eğer cihaz zaten onaysız listesinde ise
            if (_unapprovedDevices.Contains(imei))
            {
                _logger.LogInformation($"Cihaz {imei} önceden reddedilmiş");
                return false;
            }

            try
            {
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
                    _logger.LogWarning($"Cihaz {imei} bulunamadı ve reddedildi");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Cihaz doğrulama sırasında hata oluştu: {ex.Message}");
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
        public IEnumerable<DeviceInfoDto> GetApprovedDevicesWithDetails()
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                
                var devices = new List<DeviceInfoDto>();
                var allDevices = deviceRepository.GetAllDevicesAsync().GetAwaiter().GetResult();
                
                foreach (var imei in _approvedDevices)
                {
                    try
                    {
                        var device = allDevices.FirstOrDefault(d => d.IMEI == imei);
                        if (device != null)
                        {
                            devices.Add(MapToDeviceInfoDto(device, true));
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
                return Enumerable.Empty<DeviceInfoDto>();
            }
        }
        
        /// <summary>
        /// Onaysız cihazların listesini döndürür
        /// </summary>
        /// <returns>Onaysız cihazların IMEI listesi</returns>
        public IEnumerable<string> GetUnapprovedDevices()
        {
            return _unapprovedDevices.ToList();
        }
        
        /// <summary>
        /// Onaysız cihazların detaylı bilgilerini döndürür
        /// </summary>
        /// <returns>Onaysız cihazların detaylı bilgileri</returns>
        public IEnumerable<DeviceInfoDto> GetUnapprovedDevicesWithDetails()
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                
                var devices = new List<DeviceInfoDto>();
                var allDevices = deviceRepository.GetAllDevicesAsync().GetAwaiter().GetResult();
                
                foreach (var imei in _unapprovedDevices)
                {
                    try
                    {
                        var device = allDevices.FirstOrDefault(d => d.IMEI == imei);
                        if (device != null)
                        {
                            devices.Add(MapToDeviceInfoDto(device, false));
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
                _logger.LogError(ex, $"Onaysız cihazların detayları alınırken hata oluştu: {ex.Message}");
                return Enumerable.Empty<DeviceInfoDto>();
            }
        }
        
        /// <summary>
        /// Device entity'sini DeviceInfoDto'ya dönüştürür
        /// </summary>
        private DeviceInfoDto MapToDeviceInfoDto(Device device, bool isApproved)
        {
            return new DeviceInfoDto
            {
                Id = device.Id,
                Imei = device.IMEI,
                IpAddress = device.Ip,
                Port = device.Port,
                Name = device.Name,
                PlatformId = device.PlatformId,
                PlatformName = device.Platform?.Station?.Name, 
                StationName = device.Platform?.Station?.Name,
                IsApproved = isApproved,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }
    }
} 