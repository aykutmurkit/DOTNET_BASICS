using DeviceApi.Business.Services.Interfaces;
using Data.Interfaces;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Bitmap ekran mesaj servis implementasyonu
    /// </summary>
    public class BitmapScreenMessageService : IBitmapScreenMessageService
    {
        private readonly IBitmapScreenMessageRepository _bitmapScreenMessageRepository;
        private readonly IDeviceRepository _deviceRepository;

        public BitmapScreenMessageService(
            IBitmapScreenMessageRepository bitmapScreenMessageRepository,
            IDeviceRepository deviceRepository)
        {
            _bitmapScreenMessageRepository = bitmapScreenMessageRepository;
            _deviceRepository = deviceRepository;
        }

        /// <summary>
        /// Tüm bitmap ekran mesajlarını getirir
        /// </summary>
        public async Task<List<BitmapScreenMessageDto>> GetAllBitmapScreenMessagesAsync()
        {
            var messages = await _bitmapScreenMessageRepository.GetAllBitmapScreenMessagesAsync();
            return messages.Select(m => MapToDto(m)).ToList();
        }

        /// <summary>
        /// ID'ye göre bitmap ekran mesajı getirir
        /// </summary>
        public async Task<BitmapScreenMessageDto> GetBitmapScreenMessageByIdAsync(int id)
        {
            var message = await _bitmapScreenMessageRepository.GetBitmapScreenMessageByIdAsync(id);
            if (message == null)
            {
                throw new Exception("Bitmap ekran mesajı bulunamadı.");
            }

            return MapToDto(message);
        }

        /// <summary>
        /// Cihaz ID'sine göre bitmap ekran mesajı getirir
        /// </summary>
        public async Task<BitmapScreenMessageDto> GetBitmapScreenMessageByDeviceIdAsync(int deviceId)
        {
            // Cihaz var mı kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            var message = await _bitmapScreenMessageRepository.GetBitmapScreenMessageByDeviceIdAsync(deviceId);
            if (message == null)
            {
                throw new Exception("Bu cihaza ait bitmap ekran mesajı bulunamadı.");
            }

            return MapToDto(message);
        }

        /// <summary>
        /// Bitmap ekran mesajı oluşturur
        /// </summary>
        public async Task<BitmapScreenMessageDto> CreateBitmapScreenMessageAsync(CreateBitmapScreenMessageRequest request)
        {
            // Cihaz var mı kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(request.DeviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            // Cihazın zaten bitmap ekran mesajı var mı kontrol et
            var existingMessage = await _bitmapScreenMessageRepository.GetBitmapScreenMessageByDeviceIdAsync(request.DeviceId);
            if (existingMessage != null)
            {
                throw new Exception("Bu cihaza ait zaten bir bitmap ekran mesajı bulunmaktadır.");
            }

            var bitmapScreenMessage = new BitmapScreenMessage
            {
                TurkishBitmap = request.TurkishBitmap,
                EnglishBitmap = request.EnglishBitmap,
                DeviceId = request.DeviceId,
                CreatedAt = DateTime.Now
            };

            await _bitmapScreenMessageRepository.AddBitmapScreenMessageAsync(bitmapScreenMessage);
            return MapToDto(bitmapScreenMessage);
        }

        /// <summary>
        /// Bitmap ekran mesajı günceller
        /// </summary>
        public async Task<BitmapScreenMessageDto> UpdateBitmapScreenMessageAsync(int id, UpdateBitmapScreenMessageRequest request)
        {
            var bitmapScreenMessage = await _bitmapScreenMessageRepository.GetBitmapScreenMessageByIdAsync(id);
            if (bitmapScreenMessage == null)
            {
                throw new Exception("Bitmap ekran mesajı bulunamadı.");
            }

            bitmapScreenMessage.TurkishBitmap = request.TurkishBitmap;
            bitmapScreenMessage.EnglishBitmap = request.EnglishBitmap;
            bitmapScreenMessage.UpdatedAt = DateTime.Now;

            await _bitmapScreenMessageRepository.UpdateBitmapScreenMessageAsync(bitmapScreenMessage);
            return MapToDto(bitmapScreenMessage);
        }

        /// <summary>
        /// Bitmap ekran mesajı siler
        /// </summary>
        public async Task DeleteBitmapScreenMessageAsync(int id)
        {
            var bitmapScreenMessage = await _bitmapScreenMessageRepository.GetBitmapScreenMessageByIdAsync(id);
            if (bitmapScreenMessage == null)
            {
                throw new Exception("Bitmap ekran mesajı bulunamadı.");
            }

            await _bitmapScreenMessageRepository.DeleteBitmapScreenMessageAsync(id);
        }

        /// <summary>
        /// BitmapScreenMessage entity'sini BitmapScreenMessageDto'ya dönüştürür
        /// </summary>
        private BitmapScreenMessageDto MapToDto(BitmapScreenMessage bitmapScreenMessage)
        {
            return new BitmapScreenMessageDto
            {
                Id = bitmapScreenMessage.Id,
                TurkishBitmap = bitmapScreenMessage.TurkishBitmap,
                EnglishBitmap = bitmapScreenMessage.EnglishBitmap,
                CreatedAt = bitmapScreenMessage.CreatedAt,
                UpdatedAt = bitmapScreenMessage.UpdatedAt,
                DeviceId = bitmapScreenMessage.DeviceId
            };
        }
    }
} 