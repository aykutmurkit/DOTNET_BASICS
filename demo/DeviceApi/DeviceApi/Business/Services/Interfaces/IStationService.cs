using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// İstasyon servis arayüzü
    /// </summary>
    public interface IStationService
    {
        /// <summary>
        /// Tüm istasyonları sadece temel bilgilerle getirir
        /// </summary>
        Task<List<StationDto>> GetStationsBasicAsync();
        
        /// <summary>
        /// Tüm istasyonları platformları ile birlikte getirir (cihazlar olmadan)
        /// </summary>
        Task<List<StationDto>> GetStationsWithPlatformsAsync();
        
        /// <summary>
        /// Tüm istasyonları platformları ve cihazları ile birlikte getirir
        /// </summary>
        Task<List<StationDto>> GetStationsWithPlatformsAndDevicesAsync();
        
        /// <summary>
        /// Tüm istasyonları platformları ve cihazları ile birlikte getirir
        /// </summary>
        Task<List<StationDto>> GetAllStationsAsync();

        /// <summary>
        /// ID'ye göre istasyon getirir, platformları ve cihazları ile birlikte
        /// </summary>
        Task<StationDto> GetStationByIdAsync(int id);

        /// <summary>
        /// İstasyon oluşturur
        /// </summary>
        Task<StationDto> CreateStationAsync(CreateStationRequest request);

        /// <summary>
        /// İstasyon günceller
        /// </summary>
        Task<StationDto> UpdateStationAsync(int id, UpdateStationRequest request);

        /// <summary>
        /// İstasyon siler
        /// </summary>
        Task DeleteStationAsync(int id);
    }
} 