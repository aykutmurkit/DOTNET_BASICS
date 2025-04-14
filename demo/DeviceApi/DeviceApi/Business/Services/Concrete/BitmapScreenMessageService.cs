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
            var result = new List<BitmapScreenMessageDto>();
            
            foreach (var message in messages)
            {
                // Mesaja bağlı cihazları getir
                var devices = await _bitmapScreenMessageRepository.GetDevicesByBitmapScreenMessageIdAsync(message.Id);
                result.Add(MapToDto(message, devices));
            }
            
            return result;
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
            
            // Mesaja bağlı cihazları getir
            var devices = await _bitmapScreenMessageRepository.GetDevicesByBitmapScreenMessageIdAsync(id);

            return MapToDto(message, devices);
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
            
            // Mesaja bağlı cihazları getir
            var devices = await _bitmapScreenMessageRepository.GetDevicesByBitmapScreenMessageIdAsync(message.Id);

            return MapToDto(message, devices);
        }
        
        /// <summary>
        /// Bir mesaja bağlı tüm cihazları getirir
        /// </summary>
        public async Task<List<int>> GetDeviceIdsByBitmapScreenMessageIdAsync(int bitmapScreenMessageId)
        {
            // Mesajın var olduğunu kontrol et
            var message = await _bitmapScreenMessageRepository.GetBitmapScreenMessageByIdAsync(bitmapScreenMessageId);
            if (message == null)
            {
                throw new Exception("Bitmap ekran mesajı bulunamadı.");
            }
            
            // Mesaja bağlı cihazları getir
            var devices = await _bitmapScreenMessageRepository.GetDevicesByBitmapScreenMessageIdAsync(bitmapScreenMessageId);
            return devices.Select(d => d.Id).ToList();
        }

        /// <summary>
        /// Bitmap ekran mesajı oluşturur
        /// </summary>
        public async Task<BitmapScreenMessageDto> CreateBitmapScreenMessageAsync(CreateBitmapScreenMessageRequest request)
        {
            var bitmapScreenMessage = new BitmapScreenMessage
            {
                TurkishBitmap = request.TurkishBitmap,
                EnglishBitmap = request.EnglishBitmap,
                CreatedAt = DateTime.Now
            };

            await _bitmapScreenMessageRepository.AddBitmapScreenMessageAsync(bitmapScreenMessage);
            return MapToDto(bitmapScreenMessage, new List<Device>());
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
            
            // Güncellenen mesaja bağlı cihazları getir
            var devices = await _bitmapScreenMessageRepository.GetDevicesByBitmapScreenMessageIdAsync(id);
            
            return MapToDto(bitmapScreenMessage, devices);
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
        /// Cihaza mesaj atar
        /// </summary>
        public async Task AssignMessageToDeviceAsync(AssignBitmapScreenMessageRequest request)
        {
            // Cihaz ve mesajın var olduğunu kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(request.DeviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }
            
            var message = await _bitmapScreenMessageRepository.GetBitmapScreenMessageByIdAsync(request.BitmapScreenMessageId);
            if (message == null)
            {
                throw new Exception("Bitmap ekran mesajı bulunamadı.");
            }
            
            await _bitmapScreenMessageRepository.AssignMessageToDeviceAsync(request.DeviceId, request.BitmapScreenMessageId);
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
            
            await _bitmapScreenMessageRepository.UnassignMessageFromDeviceAsync(deviceId);
        }

        /// <summary>
        /// BitmapScreenMessage entity'sini BitmapScreenMessageDto'ya dönüştürür
        /// </summary>
        private BitmapScreenMessageDto MapToDto(BitmapScreenMessage bitmapScreenMessage, List<Device> devices)
        {
            return new BitmapScreenMessageDto
            {
                Id = bitmapScreenMessage.Id,
                TurkishBitmap = bitmapScreenMessage.TurkishBitmap,
                EnglishBitmap = bitmapScreenMessage.EnglishBitmap,
                Duration = bitmapScreenMessage.Duration,
                CreatedAt = bitmapScreenMessage.CreatedAt,
                UpdatedAt = bitmapScreenMessage.UpdatedAt,
                DeviceIds = devices.Select(d => d.Id).ToList()
            };
        }
    }
} 