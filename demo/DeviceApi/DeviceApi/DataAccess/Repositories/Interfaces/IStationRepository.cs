using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// İstasyon repository arayüzü
    /// </summary>
    public interface IStationRepository
    {
        /// <summary>
        /// Tüm istasyonları platformları ve cihazları ile birlikte getirir
        /// </summary>
        Task<List<Station>> GetAllStationsWithRelationsAsync();
        
        /// <summary>
        /// ID'ye göre istasyon getirir, platformları ve cihazları ile birlikte
        /// </summary>
        Task<Station> GetStationByIdWithRelationsAsync(int id);
        
        /// <summary>
        /// ID'ye göre istasyon getirir
        /// </summary>
        Task<Station> GetStationByIdAsync(int id);
        
        /// <summary>
        /// İstasyon ekler
        /// </summary>
        Task AddStationAsync(Station station);
        
        /// <summary>
        /// İstasyon günceller
        /// </summary>
        Task UpdateStationAsync(Station station);
        
        /// <summary>
        /// İstasyon siler
        /// </summary>
        Task DeleteStationAsync(int id);
        
        /// <summary>
        /// İstasyon adının zaten kullanılıp kullanılmadığını kontrol eder
        /// </summary>
        Task<bool> StationNameExistsAsync(string name);
    }
} 