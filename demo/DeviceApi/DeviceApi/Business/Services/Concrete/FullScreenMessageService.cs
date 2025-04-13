using AutoMapper;
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
        private readonly IMapper _mapper;

        public FullScreenMessageService(
            IFullScreenMessageRepository fullScreenMessageRepository,
            IDeviceRepository deviceRepository,
            IAlignmentTypeRepository alignmentTypeRepository,
            ILogService logService,
            IMapper mapper)
        {
            _fullScreenMessageRepository = fullScreenMessageRepository;
            _deviceRepository = deviceRepository;
            _alignmentTypeRepository = alignmentTypeRepository;
            _logService = logService;
            _mapper = mapper;
        }

        /// <summary>
        /// Tüm tam ekran mesajları getirir
        /// </summary>
        public async Task<List<FullScreenMessageDto>> GetAllFullScreenMessagesAsync()
        {
            var messages = await _fullScreenMessageRepository.GetAllFullScreenMessagesAsync();
            return _mapper.Map<List<FullScreenMessageDto>>(messages);
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
            
            return _mapper.Map<FullScreenMessageDto>(message);
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
            
            return _mapper.Map<FullScreenMessageDto>(message);
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
            var message = _mapper.Map<FullScreenMessage>(request);
            
            await _fullScreenMessageRepository.AddFullScreenMessageAsync(message);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj oluşturuldu", 
                "FullScreenMessageService.CreateFullScreenMessage", 
                new { MessageId = message.Id });
            
            return _mapper.Map<FullScreenMessageDto>(message);
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
            _mapper.Map(request, existingMessage);
            
            await _fullScreenMessageRepository.UpdateFullScreenMessageAsync(existingMessage);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj güncellendi", 
                "FullScreenMessageService.UpdateFullScreenMessage", 
                new { MessageId = id });
            
            return _mapper.Map<FullScreenMessageDto>(existingMessage);
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
    }
} 