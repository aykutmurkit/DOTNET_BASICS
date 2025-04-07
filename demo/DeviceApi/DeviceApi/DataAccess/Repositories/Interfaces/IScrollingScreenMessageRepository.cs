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
    }
} 