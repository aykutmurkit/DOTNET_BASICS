using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Tam ekran mesaj servis arayüzü
    /// </summary>
    public interface IFullScreenMessageService
    {
        /// <summary>
        /// Tüm tam ekran mesajları getirir
        /// </summary>
        Task<List<FullScreenMessageDto>> GetAllFullScreenMessagesAsync();

        /// <summary>
        /// ID'ye göre tam ekran mesaj getirir
        /// </summary>
        Task<FullScreenMessageDto> GetFullScreenMessageByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre tam ekran mesaj getirir
        /// </summary>
        Task<FullScreenMessageDto> GetFullScreenMessageByDeviceIdAsync(int deviceId);

        /// <summary>
        /// Cihaz için tam ekran mesaj oluşturur
        /// </summary>
        Task<FullScreenMessageDto> CreateFullScreenMessageAsync(int deviceId, CreateFullScreenMessageRequest request);

        /// <summary>
        /// Tam ekran mesaj günceller
        /// </summary>
        Task<FullScreenMessageDto> UpdateFullScreenMessageAsync(int id, UpdateFullScreenMessageRequest request);

        /// <summary>
        /// Tam ekran mesaj siler
        /// </summary>
        Task DeleteFullScreenMessageAsync(int id);
    }
} 