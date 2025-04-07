using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Kayan ekran mesaj servis arayüzü
    /// </summary>
    public interface IScrollingScreenMessageService
    {
        /// <summary>
        /// Tüm kayan ekran mesajlarını getirir
        /// </summary>
        Task<List<ScrollingScreenMessageDto>> GetAllScrollingScreenMessagesAsync();

        /// <summary>
        /// ID'ye göre kayan ekran mesajı getirir
        /// </summary>
        Task<ScrollingScreenMessageDto> GetScrollingScreenMessageByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre kayan ekran mesajı getirir
        /// </summary>
        Task<ScrollingScreenMessageDto> GetScrollingScreenMessageByDeviceIdAsync(int deviceId);

        /// <summary>
        /// Kayan ekran mesajı oluşturur
        /// </summary>
        Task<ScrollingScreenMessageDto> CreateScrollingScreenMessageAsync(CreateScrollingScreenMessageRequest request);

        /// <summary>
        /// Kayan ekran mesajı günceller
        /// </summary>
        Task<ScrollingScreenMessageDto> UpdateScrollingScreenMessageAsync(int id, UpdateScrollingScreenMessageRequest request);

        /// <summary>
        /// Kayan ekran mesajı siler
        /// </summary>
        Task DeleteScrollingScreenMessageAsync(int id);
    }
} 