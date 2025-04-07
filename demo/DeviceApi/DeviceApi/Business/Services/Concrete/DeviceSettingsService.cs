using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Cihaz ayarları servisi implementasyonu
    /// </summary>
    public class DeviceSettingsService : IDeviceSettingsService
    {
        private readonly IDeviceSettingsRepository _deviceSettingsRepository;
        private readonly IDeviceRepository _deviceRepository;

        public DeviceSettingsService(IDeviceSettingsRepository deviceSettingsRepository, IDeviceRepository deviceRepository)
        {
            _deviceSettingsRepository = deviceSettingsRepository;
            _deviceRepository = deviceRepository;
        }

        public async Task<List<DeviceSettingsDto>> GetAllDeviceSettingsAsync()
        {
            var deviceSettings = await _deviceSettingsRepository.GetAllDeviceSettingsAsync();
            return deviceSettings.Select(MapToDeviceSettingsDto).ToList();
        }

        public async Task<DeviceSettingsDto> GetDeviceSettingsByIdAsync(int id)
        {
            var deviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByIdAsync(id);
            if (deviceSettings == null)
            {
                throw new Exception("Cihaz ayarları bulunamadı.");
            }

            return MapToDeviceSettingsDto(deviceSettings);
        }
        
        public async Task<DeviceSettingsDto> GetDeviceSettingsByDeviceIdAsync(int deviceId)
        {
            var deviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByDeviceIdAsync(deviceId);
            if (deviceSettings == null)
            {
                throw new Exception("Cihaz ayarları bulunamadı.");
            }

            return MapToDeviceSettingsDto(deviceSettings);
        }

        public async Task<DeviceSettingsDto> CreateDeviceSettingsAsync(int deviceId, CreateDeviceSettingsRequest request)
        {
            // Cihaz var mı kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }
            
            // Cihazın zaten ayarları var mı kontrol et
            var existingSettings = await _deviceSettingsRepository.GetDeviceSettingsByDeviceIdAsync(deviceId);
            if (existingSettings != null)
            {
                throw new Exception("Bu cihaz için ayarlar zaten tanımlanmış.");
            }

            var deviceSettings = new DeviceSettings
            {
                ApnName = request.ApnName,
                ApnUsername = request.ApnUsername,
                ApnPassword = request.ApnPassword,
                ServerIP = request.ServerIP,
                TcpPort = request.TcpPort,
                UdpPort = request.UdpPort,
                FtpStatus = request.FtpStatus,
                DeviceId = deviceId
            };

            await _deviceSettingsRepository.AddDeviceSettingsAsync(deviceSettings);
            
            var createdDeviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByIdAsync(deviceSettings.Id);
            return MapToDeviceSettingsDto(createdDeviceSettings);
        }

        public async Task<DeviceSettingsDto> UpdateDeviceSettingsAsync(int id, UpdateDeviceSettingsRequest request)
        {
            var deviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByIdAsync(id);
            if (deviceSettings == null)
            {
                throw new Exception("Cihaz ayarları bulunamadı.");
            }

            deviceSettings.ApnName = request.ApnName;
            deviceSettings.ApnUsername = request.ApnUsername;
            deviceSettings.ApnPassword = request.ApnPassword;
            deviceSettings.ServerIP = request.ServerIP;
            deviceSettings.TcpPort = request.TcpPort;
            deviceSettings.UdpPort = request.UdpPort;
            deviceSettings.FtpStatus = request.FtpStatus;

            await _deviceSettingsRepository.UpdateDeviceSettingsAsync(deviceSettings);
            
            var updatedDeviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByIdAsync(id);
            return MapToDeviceSettingsDto(updatedDeviceSettings);
        }

        public async Task DeleteDeviceSettingsAsync(int id)
        {
            var deviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByIdAsync(id);
            if (deviceSettings == null)
            {
                throw new Exception("Cihaz ayarları bulunamadı.");
            }

            await _deviceSettingsRepository.DeleteDeviceSettingsAsync(id);
        }

        private DeviceSettingsDto MapToDeviceSettingsDto(DeviceSettings deviceSettings)
        {
            return new DeviceSettingsDto
            {
                Id = deviceSettings.Id,
                ApnName = deviceSettings.ApnName,
                ApnUsername = deviceSettings.ApnUsername,
                ApnPassword = deviceSettings.ApnPassword,
                ServerIP = deviceSettings.ServerIP,
                TcpPort = deviceSettings.TcpPort,
                UdpPort = deviceSettings.UdpPort,
                FtpStatus = deviceSettings.FtpStatus,
                DeviceId = deviceSettings.DeviceId
            };
        }
    }
} 