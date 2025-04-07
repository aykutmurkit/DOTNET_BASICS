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
            return messages.Select(m => MapToDto(m)).ToList();
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

            return MapToDto(message);
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

            return MapToDto(message);
        }

        /// <summary>
        /// Kayan ekran mesajı oluşturur
        /// </summary>
        public async Task<ScrollingScreenMessageDto> CreateScrollingScreenMessageAsync(CreateScrollingScreenMessageRequest request)
        {
            // Cihaz var mı kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(request.DeviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            // Cihazın zaten kayan ekran mesajı var mı kontrol et
            var existingMessage = await _scrollingScreenMessageRepository.GetScrollingScreenMessageByDeviceIdAsync(request.DeviceId);
            if (existingMessage != null)
            {
                throw new Exception("Bu cihaza ait zaten bir kayan ekran mesajı bulunmaktadır.");
            }

            var scrollingScreenMessage = new ScrollingScreenMessage
            {
                TurkishLine = request.TurkishLine,
                EnglishLine = request.EnglishLine,
                DeviceId = request.DeviceId,
                CreatedAt = DateTime.Now
            };

            await _scrollingScreenMessageRepository.AddScrollingScreenMessageAsync(scrollingScreenMessage);
            return MapToDto(scrollingScreenMessage);
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
            return MapToDto(scrollingScreenMessage);
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
        private ScrollingScreenMessageDto MapToDto(ScrollingScreenMessage scrollingScreenMessage)
        {
            return new ScrollingScreenMessageDto
            {
                Id = scrollingScreenMessage.Id,
                TurkishLine = scrollingScreenMessage.TurkishLine,
                EnglishLine = scrollingScreenMessage.EnglishLine,
                CreatedAt = scrollingScreenMessage.CreatedAt,
                UpdatedAt = scrollingScreenMessage.UpdatedAt,
                DeviceId = scrollingScreenMessage.DeviceId
            };
        }
    }
} 