using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Tam ekran mesaj repository arayüzü
    /// </summary>
    public interface IFullScreenMessageRepository
    {
        /// <summary>
        /// Tüm tam ekran mesajları getirir
        /// </summary>
        Task<List<FullScreenMessage>> GetAllFullScreenMessagesAsync();

        /// <summary>
        /// ID'ye göre tam ekran mesajı getirir
        /// </summary>
        Task<FullScreenMessage> GetFullScreenMessageByIdAsync(int id);

        /// <summary>
        /// Cihaz ID'sine göre tam ekran mesajı getirir
        /// </summary>
        Task<FullScreenMessage> GetFullScreenMessageByDeviceIdAsync(int deviceId);

        /// <summary>
        /// Tam ekran mesaj ekler
        /// </summary>
        Task AddFullScreenMessageAsync(FullScreenMessage fullScreenMessage);

        /// <summary>
        /// Tam ekran mesaj günceller
        /// </summary>
        Task UpdateFullScreenMessageAsync(FullScreenMessage fullScreenMessage);

        /// <summary>
        /// Tam ekran mesaj siler
        /// </summary>
        Task DeleteFullScreenMessageAsync(int id);
    }
} 