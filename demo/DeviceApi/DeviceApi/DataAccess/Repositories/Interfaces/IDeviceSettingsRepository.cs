using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Cihaz ayarları repository arayüzü
    /// </summary>
    public interface IDeviceSettingsRepository
    {
        /// <summary>
        /// Tüm cihaz ayarlarını getirir
        /// </summary>
        Task<List<DeviceSettings>> GetAllDeviceSettingsAsync();
        
        /// <summary>
        /// ID'ye göre cihaz ayarlarını getirir
        /// </summary>
        Task<DeviceSettings> GetDeviceSettingsByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre cihaz ayarlarını getirir
        /// </summary>
        Task<DeviceSettings> GetDeviceSettingsByDeviceIdAsync(int deviceId);
        
        /// <summary>
        /// Cihaz ID'sine göre cihaz ayarlarını getirir (alternatif metot ismi)
        /// </summary>
        Task<DeviceSettings> GetSettingsByDeviceIdAsync(int deviceId);
        
        /// <summary>
        /// Cihaz ayarları ekler
        /// </summary>
        Task AddDeviceSettingsAsync(DeviceSettings deviceSettings);
        
        /// <summary>
        /// Cihaz ayarlarını günceller
        /// </summary>
        Task UpdateDeviceSettingsAsync(DeviceSettings deviceSettings);
        
        /// <summary>
        /// Cihaz ayarlarını siler
        /// </summary>
        Task DeleteDeviceSettingsAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre cihaz ayarlarını siler
        /// </summary>
        Task DeleteDeviceSettingsByDeviceIdAsync(int deviceId);
    }
} 