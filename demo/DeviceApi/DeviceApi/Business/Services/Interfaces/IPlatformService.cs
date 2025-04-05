using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Platform servis arayüzü
    /// </summary>
    public interface IPlatformService
    {
        /// <summary>
        /// Tüm platformları cihazları ile birlikte getirir
        /// </summary>
        Task<List<PlatformDto>> GetAllPlatformsAsync();

        /// <summary>
        /// ID'ye göre platform getirir, cihazları ile birlikte
        /// </summary>
        Task<PlatformDto> GetPlatformByIdAsync(int id);
        
        /// <summary>
        /// İstasyon ID'sine göre platformları getirir
        /// </summary>
        Task<List<PlatformDto>> GetPlatformsByStationIdAsync(int stationId);

        /// <summary>
        /// Platform oluşturur
        /// </summary>
        Task<PlatformDto> CreatePlatformAsync(CreatePlatformRequest request);

        /// <summary>
        /// Platform günceller
        /// </summary>
        Task<PlatformDto> UpdatePlatformAsync(int id, UpdatePlatformRequest request);

        /// <summary>
        /// Platform siler
        /// </summary>
        Task DeletePlatformAsync(int id);
    }
} 