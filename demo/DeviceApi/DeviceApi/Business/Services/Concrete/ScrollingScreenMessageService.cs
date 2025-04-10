using DeviceApi.Business.Services.Interfaces;
using Data.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using AutoMapper;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Kayan ekran mesaj servis implementasyonu
    /// </summary>
    public class ScrollingScreenMessageService : IScrollingScreenMessageService
    {
        private readonly IScrollingScreenMessageRepository _scrollingScreenMessageRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;

        public ScrollingScreenMessageService(
            IScrollingScreenMessageRepository scrollingScreenMessageRepository,
            IDeviceRepository deviceRepository,
            IMapper mapper)
        {
            _scrollingScreenMessageRepository = scrollingScreenMessageRepository;
            _deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Tüm kayan ekran mesajlarını getirir
        /// </summary>
        public async Task<List<ScrollingScreenMessageDto>> GetAllScrollingScreenMessagesAsync()
        {
            var messages = await _scrollingScreenMessageRepository.GetAllScrollingScreenMessagesAsync();
            var result = new List<ScrollingScreenMessageDto>();
            
            foreach (var message in messages)
            {
                // Mesaja bağlı cihazları getir
                var devices = await _scrollingScreenMessageRepository.GetDevicesByScrollingScreenMessageIdAsync(message.Id);
                
                // AutoMapper ile dönüşüm
                var dto = _mapper.Map<ScrollingScreenMessageDto>(message);
                
                // Cihaz ID'lerini manuel olarak atama (kendi tanımladığımız DeviceIds özelliği için)
                dto.DeviceIds = devices.Select(d => d.Id).ToList();
                
                result.Add(dto);
            }
            
            return result;
        }

        /// <summary>
        /// ID'ye göre kayan ekran mesajı getirir
        /// </summary>
        public async Task<ScrollingScreenMessageDto> GetScrollingScreenMessageByIdAsync(int id)
        {
            var message = await _scrollingScreenMessageRepository.GetScrollingScreenMessageByIdAsync(id);
            if (message == null)
            {
                throw new Exception("Kayan ekran mesajı bulunamadı.");
            }
            
            // Mesaja bağlı cihazları getir
            var devices = await _scrollingScreenMessageRepository.GetDevicesByScrollingScreenMessageIdAsync(id);
            
            // AutoMapper ile dönüşüm
            var dto = _mapper.Map<ScrollingScreenMessageDto>(message);
            
            // Cihaz ID'lerini manuel olarak atama
            dto.DeviceIds = devices.Select(d => d.Id).ToList();

            return dto;
        }

        /// <summary>
        /// Cihaz ID'sine göre kayan ekran mesajı getirir
        /// </summary>
        public async Task<ScrollingScreenMessageDto> GetScrollingScreenMessageByDeviceIdAsync(int deviceId)
        {
            // Cihaz var mı kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            var message = await _scrollingScreenMessageRepository.GetScrollingScreenMessageByDeviceIdAsync(deviceId);
            if (message == null)
            {
                throw new Exception("Bu cihaza ait kayan ekran mesajı bulunamadı.");
            }
            
            // Mesaja bağlı cihazları getir
            var devices = await _scrollingScreenMessageRepository.GetDevicesByScrollingScreenMessageIdAsync(message.Id);
            
            // AutoMapper ile dönüşüm
            var dto = _mapper.Map<ScrollingScreenMessageDto>(message);
            
            // Cihaz ID'lerini manuel olarak atama
            dto.DeviceIds = devices.Select(d => d.Id).ToList();

            return dto;
        }
        
        /// <summary>
        /// Bir mesaja bağlı tüm cihazları getirir
        /// </summary>
        public async Task<List<int>> GetDeviceIdsByScrollingScreenMessageIdAsync(int scrollingScreenMessageId)
        {
            // Mesajın var olduğunu kontrol et
            var message = await _scrollingScreenMessageRepository.GetScrollingScreenMessageByIdAsync(scrollingScreenMessageId);
            if (message == null)
            {
                throw new Exception("Kayan ekran mesajı bulunamadı.");
            }
            
            // Mesaja bağlı cihazları getir
            var devices = await _scrollingScreenMessageRepository.GetDevicesByScrollingScreenMessageIdAsync(scrollingScreenMessageId);
            return devices.Select(d => d.Id).ToList();
        }

        /// <summary>
        /// Kayan ekran mesajı oluşturur
        /// </summary>
        public async Task<ScrollingScreenMessageDto> CreateScrollingScreenMessageAsync(CreateScrollingScreenMessageRequest request)
        {
            // AutoMapper ile request'ten entity oluştur
            var scrollingScreenMessage = _mapper.Map<ScrollingScreenMessage>(request);

            await _scrollingScreenMessageRepository.AddScrollingScreenMessageAsync(scrollingScreenMessage);
            
            // AutoMapper ile entity'den DTO oluştur
            var dto = _mapper.Map<ScrollingScreenMessageDto>(scrollingScreenMessage);
            dto.DeviceIds = new List<int>();
            
            return dto;
        }
        
        /// <summary>
        /// Cihaza mesaj atar
        /// </summary>
        public async Task AssignMessageToDeviceAsync(AssignScrollingScreenMessageRequest request)
        {
            // Cihaz ve mesajın var olduğunu kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(request.DeviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }
            
            var message = await _scrollingScreenMessageRepository.GetScrollingScreenMessageByIdAsync(request.ScrollingScreenMessageId);
            if (message == null)
            {
                throw new Exception("Kayan ekran mesajı bulunamadı.");
            }
            
            await _scrollingScreenMessageRepository.AssignMessageToDeviceAsync(request.DeviceId, request.ScrollingScreenMessageId);
        }
        
        /// <summary>
        /// Cihazdan mesaj bağlantısını kaldırır
        /// </summary>
        public async Task UnassignMessageFromDeviceAsync(int deviceId)
        {
            // Cihazın var olduğunu kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }
            
            await _scrollingScreenMessageRepository.UnassignMessageFromDeviceAsync(deviceId);
        }

        /// <summary>
        /// Kayan ekran mesajı günceller
        /// </summary>
        public async Task<ScrollingScreenMessageDto> UpdateScrollingScreenMessageAsync(int id, UpdateScrollingScreenMessageRequest request)
        {
            var scrollingScreenMessage = await _scrollingScreenMessageRepository.GetScrollingScreenMessageByIdAsync(id);
            if (scrollingScreenMessage == null)
            {
                throw new Exception("Kayan ekran mesajı bulunamadı.");
            }

            // AutoMapper ile güncelleme
            _mapper.Map(request, scrollingScreenMessage);
            
            // UpdatedAt'i manuel güncelle, çünkü map'te birebir mapping değil
            scrollingScreenMessage.UpdatedAt = DateTime.Now;

            await _scrollingScreenMessageRepository.UpdateScrollingScreenMessageAsync(scrollingScreenMessage);
            
            // Mesaja bağlı cihazları getir
            var devices = await _scrollingScreenMessageRepository.GetDevicesByScrollingScreenMessageIdAsync(id);
            
            // AutoMapper ile dönüşüm
            var dto = _mapper.Map<ScrollingScreenMessageDto>(scrollingScreenMessage);
            
            // Cihaz ID'lerini manuel olarak atama
            dto.DeviceIds = devices.Select(d => d.Id).ToList();
            
            return dto;
        }

        /// <summary>
        /// Kayan ekran mesajı siler
        /// </summary>
        public async Task DeleteScrollingScreenMessageAsync(int id)
        {
            var scrollingScreenMessage = await _scrollingScreenMessageRepository.GetScrollingScreenMessageByIdAsync(id);
            if (scrollingScreenMessage == null)
            {
                throw new Exception("Kayan ekran mesajı bulunamadı.");
            }

            await _scrollingScreenMessageRepository.DeleteScrollingScreenMessageAsync(id);
        }
    }
} 