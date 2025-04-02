using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Cihaz repository arayüzü
    /// </summary>
    public interface IDeviceRepository
    {
        /// <summary>
        /// Tüm cihazları getirir
        /// </summary>
        Task<List<Device>> GetAllDevicesAsync();
        
        /// <summary>
        /// ID'ye göre cihaz getirir
        /// </summary>
        Task<Device> GetDeviceByIdAsync(int id);
        
        /// <summary>
        /// Platform ID'sine göre cihazları getirir
        /// </summary>
        Task<List<Device>> GetDevicesByPlatformIdAsync(int platformId);
        
        /// <summary>
        /// İstasyon ID'sine göre dolaylı yoldan bağlı tüm cihazları getirir
        /// </summary>
        Task<List<Device>> GetDevicesByStationIdAsync(int stationId);
        
        /// <summary>
        /// Cihaz ekler
        /// </summary>
        Task AddDeviceAsync(Device device);
        
        /// <summary>
        /// Cihaz günceller
        /// </summary>
        Task UpdateDeviceAsync(Device device);
        
        /// <summary>
        /// Cihaz siler
        /// </summary>
        Task DeleteDeviceAsync(int id);
        
        /// <summary>
        /// Aynı IP ve port ile başka bir cihaz var mı kontrol eder
        /// </summary>
        Task<bool> IpPortCombinationExistsAsync(string ip, int port, int? excludeDeviceId = null);
    }
} 