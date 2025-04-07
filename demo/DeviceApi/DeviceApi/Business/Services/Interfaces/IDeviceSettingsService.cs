using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Cihaz ayarları servis arayüzü
    /// </summary>
    public interface IDeviceSettingsService
    {
        /// <summary>
        /// Tüm cihaz ayarlarını getirir
        /// </summary>
        Task<List<DeviceSettingsDto>> GetAllDeviceSettingsAsync();

        /// <summary>
        /// ID'ye göre cihaz ayarlarını getirir
        /// </summary>
        Task<DeviceSettingsDto> GetDeviceSettingsByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre cihaz ayarlarını getirir
        /// </summary>
        Task<DeviceSettingsDto> GetDeviceSettingsByDeviceIdAsync(int deviceId);

        /// <summary>
        /// Cihaz ayarları oluşturur
        /// </summary>
        Task<DeviceSettingsDto> CreateDeviceSettingsAsync(int deviceId, CreateDeviceSettingsRequest request);

        /// <summary>
        /// Cihaz ayarlarını günceller
        /// </summary>
        Task<DeviceSettingsDto> UpdateDeviceSettingsAsync(int id, UpdateDeviceSettingsRequest request);

        /// <summary>
        /// Cihaz ayarlarını siler
        /// </summary>
        Task DeleteDeviceSettingsAsync(int id);
    }
} 