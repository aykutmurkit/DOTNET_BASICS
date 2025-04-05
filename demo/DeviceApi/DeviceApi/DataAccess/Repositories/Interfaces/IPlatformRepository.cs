using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Platform repository arayüzü
    /// </summary>
    public interface IPlatformRepository
    {
        /// <summary>
        /// Tüm platformları cihazları ile birlikte getirir
        /// </summary>
        Task<List<Platform>> GetAllPlatformsWithDevicesAsync();
        
        /// <summary>
        /// ID'ye göre platform getirir, cihazları ile birlikte
        /// </summary>
        Task<Platform> GetPlatformByIdWithDevicesAsync(int id);
        
        /// <summary>
        /// ID'ye göre platform getirir
        /// </summary>
        Task<Platform> GetPlatformByIdAsync(int id);
        
        /// <summary>
        /// İstasyon ID'sine göre platformları getirir
        /// </summary>
        Task<List<Platform>> GetPlatformsByStationIdAsync(int stationId);
        
        /// <summary>
        /// Platform ekler
        /// </summary>
        Task AddPlatformAsync(Platform platform);
        
        /// <summary>
        /// Platform günceller
        /// </summary>
        Task UpdatePlatformAsync(Platform platform);
        
        /// <summary>
        /// Platform siler
        /// </summary>
        Task DeletePlatformAsync(int id);
    }
} 