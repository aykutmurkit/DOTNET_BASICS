using DeviceApi.Business.Services.Interfaces;
using Data.Interfaces;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Kayan ekran mesaj servis implementasyonu
    /// </summary>
    public class ScrollingScreenMessageService : IScrollingScreenMessageService
    {
        private readonly IScrollingScreenMessageRepository _scrollingScreenMessageRepository;
        private readonly IDeviceRepository _deviceRepository;

        public ScrollingScreenMessageService(
            IScrollingScreenMessageRepository scrollingScreenMessageRepository,
            IDeviceRepository deviceRepository)
        {
            _scrollingScreenMessageRepository = scrollingScreenMessageRepository;
            _deviceRepository = deviceRepository;
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
                result.Add(MapToDto(message, devices));
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

            return MapToDto(message, devices);
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

            return MapToDto(message, devices);
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
            var scrollingScreenMessage = new ScrollingScreenMessage
            {
                TurkishLine = request.TurkishLine,
                EnglishLine = request.EnglishLine,
                CreatedAt = DateTime.Now
            };

            await _scrollingScreenMessageRepository.AddScrollingScreenMessageAsync(scrollingScreenMessage);
            return MapToDto(scrollingScreenMessage, new List<Device>());
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

            scrollingScreenMessage.TurkishLine = request.TurkishLine;
            scrollingScreenMessage.EnglishLine = request.EnglishLine;
            scrollingScreenMessage.UpdatedAt = DateTime.Now;

            await _scrollingScreenMessageRepository.UpdateScrollingScreenMessageAsync(scrollingScreenMessage);
            
            // Mesaja bağlı cihazları getir
            var devices = await _scrollingScreenMessageRepository.GetDevicesByScrollingScreenMessageIdAsync(id);
            
            return MapToDto(scrollingScreenMessage, devices);
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

        /// <summary>
        /// ScrollingScreenMessage entity'sini ScrollingScreenMessageDto'ya dönüştürür
        /// </summary>
        private ScrollingScreenMessageDto MapToDto(ScrollingScreenMessage scrollingScreenMessage, List<Device> devices)
        {
            return new ScrollingScreenMessageDto
            {
                Id = scrollingScreenMessage.Id,
                TurkishLine = scrollingScreenMessage.TurkishLine,
                EnglishLine = scrollingScreenMessage.EnglishLine,
                CreatedAt = scrollingScreenMessage.CreatedAt,
                UpdatedAt = scrollingScreenMessage.UpdatedAt,
                DeviceIds = devices.Select(d => d.Id).ToList()
            };
        }
    }
} 