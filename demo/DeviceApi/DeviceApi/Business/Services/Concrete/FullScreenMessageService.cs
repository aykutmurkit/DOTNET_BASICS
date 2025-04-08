using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using LogLibrary.Core.Interfaces;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Tam ekran mesaj servis implementasyonu
    /// </summary>
    public class FullScreenMessageService : IFullScreenMessageService
    {
        private readonly IFullScreenMessageRepository _fullScreenMessageRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILogService _logService;

        public FullScreenMessageService(
            IFullScreenMessageRepository fullScreenMessageRepository,
            IDeviceRepository deviceRepository,
            ILogService logService)
        {
            _fullScreenMessageRepository = fullScreenMessageRepository;
            _deviceRepository = deviceRepository;
            _logService = logService;
        }

        /// <summary>
        /// Tüm tam ekran mesajları getirir
        /// </summary>
        public async Task<List<FullScreenMessageDto>> GetAllFullScreenMessagesAsync()
        {
            var messages = await _fullScreenMessageRepository.GetAllFullScreenMessagesAsync();
            return messages.Select(MapToFullScreenMessageDto).ToList();
        }

        /// <summary>
        /// ID'ye göre tam ekran mesaj getirir
        /// </summary>
        public async Task<FullScreenMessageDto> GetFullScreenMessageByIdAsync(int id)
        {
            var message = await _fullScreenMessageRepository.GetFullScreenMessageByIdAsync(id);
            if (message == null)
            {
                throw new Exception("Tam ekran mesaj bulunamadı");
            }
            
            return MapToFullScreenMessageDto(message);
        }
        
        /// <summary>
        /// Cihaz ID'sine göre tam ekran mesaj getirir
        /// </summary>
        public async Task<FullScreenMessageDto> GetFullScreenMessageByDeviceIdAsync(int deviceId)
        {
            // Önce cihazın var olduğunu kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı");
            }
            
            var message = await _fullScreenMessageRepository.GetFullScreenMessageByDeviceIdAsync(deviceId);
            if (message == null)
            {
                throw new Exception("Cihaz için tam ekran mesaj bulunamadı");
            }
            
            return MapToFullScreenMessageDto(message);
        }

        /// <summary>
        /// Cihaz için tam ekran mesaj oluşturur
        /// </summary>
        public async Task<FullScreenMessageDto> CreateFullScreenMessageAsync(int deviceId, CreateFullScreenMessageRequest request)
        {
            // Cihaz var mı kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı");
            }
            
            // Cihaz için zaten bir mesaj var mı kontrol et
            var existingMessage = await _fullScreenMessageRepository.GetFullScreenMessageByDeviceIdAsync(deviceId);
            if (existingMessage != null)
            {
                throw new Exception("Bu cihaz için zaten bir tam ekran mesaj mevcut. Güncelleme yapabilirsiniz.");
            }
            
            // Yeni mesaj oluştur
            var message = new FullScreenMessage
            {
                TurkishLine1 = request.TurkishLine1,
                TurkishLine2 = request.TurkishLine2,
                TurkishLine3 = request.TurkishLine3,
                TurkishLine4 = request.TurkishLine4,
                EnglishLine1 = request.EnglishLine1,
                EnglishLine2 = request.EnglishLine2,
                EnglishLine3 = request.EnglishLine3,
                EnglishLine4 = request.EnglishLine4,
                CreatedAt = DateTime.Now,
                DeviceId = deviceId
            };
            
            await _fullScreenMessageRepository.AddFullScreenMessageAsync(message);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj oluşturuldu", 
                "FullScreenMessageService.CreateFullScreenMessage", 
                new { MessageId = message.Id, DeviceId = deviceId });
            
            return MapToFullScreenMessageDto(message);
        }

        /// <summary>
        /// Tam ekran mesaj günceller
        /// </summary>
        public async Task<FullScreenMessageDto> UpdateFullScreenMessageAsync(int id, UpdateFullScreenMessageRequest request)
        {
            // Mesaj var mı kontrol et
            var existingMessage = await _fullScreenMessageRepository.GetFullScreenMessageByIdAsync(id);
            if (existingMessage == null)
            {
                throw new Exception("Güncellenecek tam ekran mesaj bulunamadı");
            }
            
            // Mesajı güncelle
            existingMessage.TurkishLine1 = request.TurkishLine1;
            existingMessage.TurkishLine2 = request.TurkishLine2;
            existingMessage.TurkishLine3 = request.TurkishLine3;
            existingMessage.TurkishLine4 = request.TurkishLine4;
            existingMessage.EnglishLine1 = request.EnglishLine1;
            existingMessage.EnglishLine2 = request.EnglishLine2;
            existingMessage.EnglishLine3 = request.EnglishLine3;
            existingMessage.EnglishLine4 = request.EnglishLine4;
            existingMessage.ModifiedAt = DateTime.Now;
            
            await _fullScreenMessageRepository.UpdateFullScreenMessageAsync(existingMessage);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj güncellendi", 
                "FullScreenMessageService.UpdateFullScreenMessage", 
                new { MessageId = id });
            
            return MapToFullScreenMessageDto(existingMessage);
        }

        /// <summary>
        /// Tam ekran mesaj siler
        /// </summary>
        public async Task DeleteFullScreenMessageAsync(int id)
        {
            // Mesaj var mı kontrol et
            var existingMessage = await _fullScreenMessageRepository.GetFullScreenMessageByIdAsync(id);
            if (existingMessage == null)
            {
                throw new Exception("Silinecek tam ekran mesaj bulunamadı");
            }
            
            await _fullScreenMessageRepository.DeleteFullScreenMessageAsync(id);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj silindi", 
                "FullScreenMessageService.DeleteFullScreenMessage", 
                new { MessageId = id });
        }

        /// <summary>
        /// Entity'den DTO'ya dönüşüm
        /// </summary>
        private FullScreenMessageDto MapToFullScreenMessageDto(FullScreenMessage message)
        {
            return new FullScreenMessageDto
            {
                Id = message.Id,
                TurkishLine1 = message.TurkishLine1,
                TurkishLine2 = message.TurkishLine2,
                TurkishLine3 = message.TurkishLine3,
                TurkishLine4 = message.TurkishLine4,
                EnglishLine1 = message.EnglishLine1,
                EnglishLine2 = message.EnglishLine2,
                EnglishLine3 = message.EnglishLine3,
                EnglishLine4 = message.EnglishLine4,
                DeviceId = message.DeviceId,
                CreatedAt = message.CreatedAt,
                ModifiedAt = message.ModifiedAt
            };
        }
    }
} 