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
        private readonly IAlignmentTypeRepository _alignmentTypeRepository;
        private readonly ILogService _logService;

        public FullScreenMessageService(
            IFullScreenMessageRepository fullScreenMessageRepository,
            IDeviceRepository deviceRepository,
            IAlignmentTypeRepository alignmentTypeRepository,
            ILogService logService)
        {
            _fullScreenMessageRepository = fullScreenMessageRepository;
            _deviceRepository = deviceRepository;
            _alignmentTypeRepository = alignmentTypeRepository;
            _logService = logService;
        }

        /// <summary>
        /// Tüm tam ekran mesajları getirir
        /// </summary>
        public async Task<List<FullScreenMessageDto>> GetAllFullScreenMessagesAsync()
        {
            var messages = await _fullScreenMessageRepository.GetAllFullScreenMessagesAsync();
            
            // Her mesaj için AlignmentType bilgisini ve cihaz listesini yükle
            var result = new List<FullScreenMessageDto>();
            foreach (var message in messages)
            {
                // Mesaja bağlı cihazları getir
                var devices = await _fullScreenMessageRepository.GetDevicesByFullScreenMessageIdAsync(message.Id);
                
                // DTO'ya dönüştür
                result.Add(MapToFullScreenMessageDto(message, message.AlignmentType, devices));
            }
            
            return result;
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
            
            // Mesaja bağlı cihazları getir
            var devices = await _fullScreenMessageRepository.GetDevicesByFullScreenMessageIdAsync(message.Id);
            
            return MapToFullScreenMessageDto(message, message.AlignmentType, devices);
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
            
            // Mesaja bağlı cihazları getir
            var devices = await _fullScreenMessageRepository.GetDevicesByFullScreenMessageIdAsync(message.Id);
            
            return MapToFullScreenMessageDto(message, message.AlignmentType, devices);
        }
        
        /// <summary>
        /// Bir mesaja bağlı tüm cihazları getirir
        /// </summary>
        public async Task<List<int>> GetDeviceIdsByFullScreenMessageIdAsync(int fullScreenMessageId)
        {
            // Mesajın var olduğunu kontrol et
            var message = await _fullScreenMessageRepository.GetFullScreenMessageByIdAsync(fullScreenMessageId);
            if (message == null)
            {
                throw new Exception("Tam ekran mesaj bulunamadı");
            }
            
            // Mesaja bağlı cihazları getir
            var devices = await _fullScreenMessageRepository.GetDevicesByFullScreenMessageIdAsync(fullScreenMessageId);
            return devices.Select(d => d.Id).ToList();
        }

        /// <summary>
        /// Yeni bir tam ekran mesaj oluşturur
        /// </summary>
        public async Task<FullScreenMessageDto> CreateFullScreenMessageAsync(CreateFullScreenMessageRequest request)
        {
            // Alignment type kontrolü
            var alignmentType = await _alignmentTypeRepository.GetAlignmentTypeByIdAsync(request.AlignmentTypeId);
            if (alignmentType == null)
            {
                throw new Exception("Belirtilen hizalama türü bulunamadı");
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
                AlignmentTypeId = request.AlignmentTypeId,
                CreatedAt = DateTime.Now
            };
            
            await _fullScreenMessageRepository.AddFullScreenMessageAsync(message);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj oluşturuldu", 
                "FullScreenMessageService.CreateFullScreenMessage", 
                new { MessageId = message.Id });
            
            return MapToFullScreenMessageDto(message, alignmentType, new List<Device>());
        }
        
        /// <summary>
        /// Cihaza mesaj atar
        /// </summary>
        public async Task AssignMessageToDeviceAsync(AssignFullScreenMessageRequest request)
        {
            await _fullScreenMessageRepository.AssignMessageToDeviceAsync(request.DeviceId, request.FullScreenMessageId);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj cihaza atandı", 
                "FullScreenMessageService.AssignMessageToDevice", 
                new { DeviceId = request.DeviceId, MessageId = request.FullScreenMessageId });
        }
        
        /// <summary>
        /// Cihazdan mesaj bağlantısını kaldırır
        /// </summary>
        public async Task UnassignMessageFromDeviceAsync(int deviceId)
        {
            await _fullScreenMessageRepository.UnassignMessageFromDeviceAsync(deviceId);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj cihazdan kaldırıldı", 
                "FullScreenMessageService.UnassignMessageFromDevice", 
                new { DeviceId = deviceId });
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
            
            // Alignment type kontrolü
            var alignmentType = await _alignmentTypeRepository.GetAlignmentTypeByIdAsync(request.AlignmentTypeId);
            if (alignmentType == null)
            {
                throw new Exception("Belirtilen hizalama türü bulunamadı");
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
            existingMessage.AlignmentTypeId = request.AlignmentTypeId;
            existingMessage.ModifiedAt = DateTime.Now;
            
            await _fullScreenMessageRepository.UpdateFullScreenMessageAsync(existingMessage);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj güncellendi", 
                "FullScreenMessageService.UpdateFullScreenMessage", 
                new { MessageId = id });
            
            // Mesaja bağlı cihazları getir
            var devices = await _fullScreenMessageRepository.GetDevicesByFullScreenMessageIdAsync(id);
            
            return MapToFullScreenMessageDto(existingMessage, alignmentType, devices);
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
        private FullScreenMessageDto MapToFullScreenMessageDto(FullScreenMessage message, AlignmentType alignmentType, List<Device> devices)
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
                Alignment = alignmentType != null ? new AlignmentValueDto
                {
                    Id = alignmentType.Id,
                    Key = alignmentType.Key,
                    Name = alignmentType.Name
                } : null,
                DeviceIds = devices.Select(d => d.Id).ToList(),
                CreatedAt = message.CreatedAt,
                ModifiedAt = message.ModifiedAt
            };
        }
    }
} 