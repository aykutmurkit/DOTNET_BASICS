using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Periyodik mesaj servis arayüzü
    /// </summary>
    public interface IPeriodicMessageService
    {
        /// <summary>
        /// Tüm periyodik mesajları getirir
        /// </summary>
        Task<List<PeriodicMessageDto>> GetAllPeriodicMessagesAsync();

        /// <summary>
        /// ID'ye göre periyodik mesajı getirir
        /// </summary>
        Task<PeriodicMessageDto> GetPeriodicMessageByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre periyodik mesajı getirir
        /// </summary>
        Task<PeriodicMessageDto> GetPeriodicMessageByDeviceIdAsync(int deviceId);

        /// <summary>
        /// Periyodik mesaj oluşturur
        /// </summary>
        Task<PeriodicMessageDto> CreatePeriodicMessageAsync(CreatePeriodicMessageRequest request);

        /// <summary>
        /// Periyodik mesaj günceller
        /// </summary>
        Task<PeriodicMessageDto> UpdatePeriodicMessageAsync(int id, UpdatePeriodicMessageRequest request);

        /// <summary>
        /// Periyodik mesaj siler
        /// </summary>
        Task DeletePeriodicMessageAsync(int id);
    }
} 