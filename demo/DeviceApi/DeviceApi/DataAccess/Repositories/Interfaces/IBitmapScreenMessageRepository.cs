using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Bitmap ekran mesaj repository arayüzü
    /// </summary>
    public interface IBitmapScreenMessageRepository
    {
        /// <summary>
        /// Tüm bitmap ekran mesajlarını getirir
        /// </summary>
        Task<List<BitmapScreenMessage>> GetAllBitmapScreenMessagesAsync();
        
        /// <summary>
        /// ID'ye göre bitmap ekran mesajı getirir
        /// </summary>
        Task<BitmapScreenMessage> GetBitmapScreenMessageByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre bitmap ekran mesajı getirir
        /// </summary>
        Task<BitmapScreenMessage> GetBitmapScreenMessageByDeviceIdAsync(int deviceId);
        
        /// <summary>
        /// Bir mesaja bağlı tüm cihazları getirir
        /// </summary>
        Task<List<Device>> GetDevicesByBitmapScreenMessageIdAsync(int bitmapScreenMessageId);
        
        /// <summary>
        /// Bitmap ekran mesajı ekler
        /// </summary>
        Task AddBitmapScreenMessageAsync(BitmapScreenMessage bitmapScreenMessage);
        
        /// <summary>
        /// Bitmap ekran mesajı günceller
        /// </summary>
        Task UpdateBitmapScreenMessageAsync(BitmapScreenMessage bitmapScreenMessage);
        
        /// <summary>
        /// Bitmap ekran mesajı siler
        /// </summary>
        Task DeleteBitmapScreenMessageAsync(int id);
        
        /// <summary>
        /// Cihaza mesaj atar
        /// </summary>
        Task AssignMessageToDeviceAsync(int deviceId, int bitmapScreenMessageId);
        
        /// <summary>
        /// Cihazdan mesaj bağlantısını kaldırır
        /// </summary>
        Task UnassignMessageFromDeviceAsync(int deviceId);
    }
} 