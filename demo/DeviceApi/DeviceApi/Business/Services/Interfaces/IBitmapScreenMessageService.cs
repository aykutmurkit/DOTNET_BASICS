using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Bitmap ekran mesaj servis arayüzü
    /// </summary>
    public interface IBitmapScreenMessageService
    {
        /// <summary>
        /// Tüm bitmap ekran mesajlarını getirir
        /// </summary>
        Task<List<BitmapScreenMessageDto>> GetAllBitmapScreenMessagesAsync();

        /// <summary>
        /// ID'ye göre bitmap ekran mesajı getirir
        /// </summary>
        Task<BitmapScreenMessageDto> GetBitmapScreenMessageByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre bitmap ekran mesajı getirir
        /// </summary>
        Task<BitmapScreenMessageDto> GetBitmapScreenMessageByDeviceIdAsync(int deviceId);
        
        /// <summary>
        /// Bir mesaja bağlı tüm cihazları getirir
        /// </summary>
        Task<List<int>> GetDeviceIdsByBitmapScreenMessageIdAsync(int bitmapScreenMessageId);

        /// <summary>
        /// Bitmap ekran mesajı oluşturur
        /// </summary>
        Task<BitmapScreenMessageDto> CreateBitmapScreenMessageAsync(CreateBitmapScreenMessageRequest request);

        /// <summary>
        /// Bitmap ekran mesajı günceller
        /// </summary>
        Task<BitmapScreenMessageDto> UpdateBitmapScreenMessageAsync(int id, UpdateBitmapScreenMessageRequest request);

        /// <summary>
        /// Bitmap ekran mesajı siler
        /// </summary>
        Task DeleteBitmapScreenMessageAsync(int id);
        
        /// <summary>
        /// Cihaza mesaj atar
        /// </summary>
        Task AssignMessageToDeviceAsync(AssignBitmapScreenMessageRequest request);
        
        /// <summary>
        /// Cihazdan mesaj bağlantısını kaldırır
        /// </summary>
        Task UnassignMessageFromDeviceAsync(int deviceId);
    }
} 