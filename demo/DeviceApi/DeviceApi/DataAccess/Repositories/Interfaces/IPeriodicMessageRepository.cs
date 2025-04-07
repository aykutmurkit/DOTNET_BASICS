using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Periyodik mesaj repository arayüzü
    /// </summary>
    public interface IPeriodicMessageRepository
    {
        /// <summary>
        /// Tüm periyodik mesajları getirir
        /// </summary>
        Task<List<PeriodicMessage>> GetAllPeriodicMessagesAsync();
        
        /// <summary>
        /// ID'ye göre periyodik mesajı getirir
        /// </summary>
        Task<PeriodicMessage> GetPeriodicMessageByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre periyodik mesajı getirir
        /// </summary>
        Task<PeriodicMessage> GetPeriodicMessageByDeviceIdAsync(int deviceId);
        
        /// <summary>
        /// Periyodik mesaj ekler
        /// </summary>
        Task AddPeriodicMessageAsync(PeriodicMessage periodicMessage);
        
        /// <summary>
        /// Periyodik mesaj günceller
        /// </summary>
        Task UpdatePeriodicMessageAsync(PeriodicMessage periodicMessage);
        
        /// <summary>
        /// Periyodik mesaj siler
        /// </summary>
        Task DeletePeriodicMessageAsync(int id);
    }
} 