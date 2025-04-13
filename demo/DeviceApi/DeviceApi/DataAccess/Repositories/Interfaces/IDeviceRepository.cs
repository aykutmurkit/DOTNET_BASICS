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
        /// ID'ye göre cihaz getirir (alternatif metot ismi)
        /// </summary>
        Task<Device> GetByIdAsync(int id);
        
        /// <summary>
        /// İsme göre cihazları filtreler
        /// </summary>
        Task<List<Device>> GetDevicesByNameAsync(string name);
        
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
        
        /// <summary>
        /// Belirtilen cihaz dışında, aynı IP ve port ile başka bir cihaz var mı kontrol eder
        /// </summary>
        Task<bool> IpPortCombinationExistsForDifferentDeviceAsync(int deviceId, string ip, int port);
    }
} 