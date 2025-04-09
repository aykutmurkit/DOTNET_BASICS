using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Kayan ekran mesaj repository arayüzü
    /// </summary>
    public interface IScrollingScreenMessageRepository
    {
        /// <summary>
        /// Tüm kayan ekran mesajlarını getirir
        /// </summary>
        Task<List<ScrollingScreenMessage>> GetAllScrollingScreenMessagesAsync();
        
        /// <summary>
        /// ID'ye göre kayan ekran mesajı getirir
        /// </summary>
        Task<ScrollingScreenMessage> GetScrollingScreenMessageByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre kayan ekran mesajı getirir
        /// </summary>
        Task<ScrollingScreenMessage> GetScrollingScreenMessageByDeviceIdAsync(int deviceId);
        
        /// <summary>
        /// Bir mesaja bağlı tüm cihazları getirir
        /// </summary>
        Task<List<Device>> GetDevicesByScrollingScreenMessageIdAsync(int scrollingScreenMessageId);
        
        /// <summary>
        /// Kayan ekran mesajı ekler
        /// </summary>
        Task AddScrollingScreenMessageAsync(ScrollingScreenMessage scrollingScreenMessage);
        
        /// <summary>
        /// Kayan ekran mesajı günceller
        /// </summary>
        Task UpdateScrollingScreenMessageAsync(ScrollingScreenMessage scrollingScreenMessage);
        
        /// <summary>
        /// Kayan ekran mesajı siler
        /// </summary>
        Task DeleteScrollingScreenMessageAsync(int id);
        
        /// <summary>
        /// Cihaza mesaj atar
        /// </summary>
        Task AssignMessageToDeviceAsync(int deviceId, int scrollingScreenMessageId);
        
        /// <summary>
        /// Cihazdan mesaj bağlantısını kaldırır
        /// </summary>
        Task UnassignMessageFromDeviceAsync(int deviceId);
    }
} 