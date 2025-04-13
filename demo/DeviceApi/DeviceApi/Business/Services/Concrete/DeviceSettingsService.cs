using AutoMapper;
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
        private readonly IMapper _mapper;

        public DeviceSettingsService(
            IDeviceSettingsRepository deviceSettingsRepository, 
            IDeviceRepository deviceRepository,
            IMapper mapper)
        {
            _deviceSettingsRepository = deviceSettingsRepository;
            _deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        public async Task<List<DeviceSettingsDto>> GetAllDeviceSettingsAsync()
        {
            var deviceSettings = await _deviceSettingsRepository.GetAllDeviceSettingsAsync();
            return _mapper.Map<List<DeviceSettingsDto>>(deviceSettings);
        }

        public async Task<DeviceSettingsDto> GetDeviceSettingsByIdAsync(int id)
        {
            var deviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByIdAsync(id);
            if (deviceSettings == null)
            {
                throw new Exception("Cihaz ayarları bulunamadı.");
            }

            return _mapper.Map<DeviceSettingsDto>(deviceSettings);
        }
        
        public async Task<DeviceSettingsDto> GetDeviceSettingsByDeviceIdAsync(int deviceId)
        {
            var deviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByDeviceIdAsync(deviceId);
            if (deviceSettings == null)
            {
                throw new Exception("Cihaz ayarları bulunamadı.");
            }

            return _mapper.Map<DeviceSettingsDto>(deviceSettings);
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

            var deviceSettings = _mapper.Map<DeviceSettings>(request);
            deviceSettings.DeviceId = deviceId;

            await _deviceSettingsRepository.AddDeviceSettingsAsync(deviceSettings);
            
            var createdDeviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByIdAsync(deviceSettings.Id);
            return _mapper.Map<DeviceSettingsDto>(createdDeviceSettings);
        }

        public async Task<DeviceSettingsDto> UpdateDeviceSettingsAsync(int id, UpdateDeviceSettingsRequest request)
        {
            var deviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByIdAsync(id);
            if (deviceSettings == null)
            {
                throw new Exception("Cihaz ayarları bulunamadı.");
            }

            _mapper.Map(request, deviceSettings);

            await _deviceSettingsRepository.UpdateDeviceSettingsAsync(deviceSettings);
            
            var updatedDeviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByIdAsync(id);
            return _mapper.Map<DeviceSettingsDto>(updatedDeviceSettings);
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
    }
} 