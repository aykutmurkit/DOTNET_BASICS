using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Cihaz durum repository arayüzü
    /// </summary>
    public interface IDeviceStatusRepository
    {
        /// <summary>
        /// Tüm cihaz durumlarını getirir
        /// </summary>
        Task<List<DeviceStatus>> GetAllDeviceStatusesAsync();
        
        /// <summary>
        /// ID'ye göre cihaz durumu getirir
        /// </summary>
        Task<DeviceStatus> GetDeviceStatusByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre cihaz durumu getirir
        /// </summary>
        Task<DeviceStatus> GetDeviceStatusByDeviceIdAsync(int deviceId);
        
        /// <summary>
        /// Yeni cihaz durumu ekler
        /// </summary>
        Task AddDeviceStatusAsync(DeviceStatus deviceStatus);
        
        /// <summary>
        /// Cihaz durumunu günceller
        /// </summary>
        Task UpdateDeviceStatusAsync(DeviceStatus deviceStatus);
        
        /// <summary>
        /// Cihaz durumunu siler
        /// </summary>
        Task DeleteDeviceStatusAsync(int id);
    }
} 