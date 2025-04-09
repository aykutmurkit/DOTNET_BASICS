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
        /// Bir mesaja bağlı tüm cihazları getirir
        /// </summary>
        Task<List<int>> GetDeviceIdsByFullScreenMessageIdAsync(int fullScreenMessageId);

        /// <summary>
        /// Yeni bir tam ekran mesaj oluşturur
        /// </summary>
        Task<FullScreenMessageDto> CreateFullScreenMessageAsync(CreateFullScreenMessageRequest request);
        
        /// <summary>
        /// Cihaza mesaj atar
        /// </summary>
        Task AssignMessageToDeviceAsync(AssignFullScreenMessageRequest request);
        
        /// <summary>
        /// Cihazdan mesaj bağlantısını kaldırır
        /// </summary>
        Task UnassignMessageFromDeviceAsync(int deviceId);

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