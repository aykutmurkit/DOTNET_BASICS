using AutoMapper;
using DeviceApi.Business.Services.Interfaces;
using Data.Interfaces;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Periyodik mesaj servis implementasyonu
    /// </summary>
    public class PeriodicMessageService : IPeriodicMessageService
    {
        private readonly IPeriodicMessageRepository _periodicMessageRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;

        public PeriodicMessageService(
            IPeriodicMessageRepository periodicMessageRepository,
            IDeviceRepository deviceRepository,
            IMapper mapper)
        {
            _periodicMessageRepository = periodicMessageRepository;
            _deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Tüm periyodik mesajları getirir
        /// </summary>
        public async Task<List<PeriodicMessageDto>> GetAllPeriodicMessagesAsync()
        {
            var messages = await _periodicMessageRepository.GetAllPeriodicMessagesAsync();
            return _mapper.Map<List<PeriodicMessageDto>>(messages);
        }

        /// <summary>
        /// ID'ye göre periyodik mesajı getirir
        /// </summary>
        public async Task<PeriodicMessageDto> GetPeriodicMessageByIdAsync(int id)
        {
            var message = await _periodicMessageRepository.GetPeriodicMessageByIdAsync(id);
            if (message == null)
            {
                throw new Exception("Periyodik mesaj bulunamadı.");
            }

            return _mapper.Map<PeriodicMessageDto>(message);
        }

        /// <summary>
        /// Cihaz ID'sine göre periyodik mesajı getirir
        /// </summary>
        public async Task<PeriodicMessageDto> GetPeriodicMessageByDeviceIdAsync(int deviceId)
        {
            // Cihaz var mı kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            var message = await _periodicMessageRepository.GetPeriodicMessageByDeviceIdAsync(deviceId);
            if (message == null)
            {
                throw new Exception("Bu cihaza ait periyodik mesaj bulunamadı.");
            }

            return _mapper.Map<PeriodicMessageDto>(message);
        }

        /// <summary>
        /// Periyodik mesaj oluşturur
        /// </summary>
        public async Task<PeriodicMessageDto> CreatePeriodicMessageAsync(CreatePeriodicMessageRequest request)
        {
            // Cihaz var mı kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(request.DeviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            // Cihazın zaten periyodik mesajı var mı kontrol et
            var existingMessage = await _periodicMessageRepository.GetPeriodicMessageByDeviceIdAsync(request.DeviceId);
            if (existingMessage != null)
            {
                throw new Exception("Bu cihaza ait zaten bir periyodik mesaj bulunmaktadır.");
            }

            var periodicMessage = _mapper.Map<PeriodicMessage>(request);

            await _periodicMessageRepository.AddPeriodicMessageAsync(periodicMessage);
            return _mapper.Map<PeriodicMessageDto>(periodicMessage);
        }

        /// <summary>
        /// Periyodik mesaj günceller
        /// </summary>
        public async Task<PeriodicMessageDto> UpdatePeriodicMessageAsync(int id, UpdatePeriodicMessageRequest request)
        {
            var periodicMessage = await _periodicMessageRepository.GetPeriodicMessageByIdAsync(id);
            if (periodicMessage == null)
            {
                throw new Exception("Periyodik mesaj bulunamadı.");
            }

            _mapper.Map(request, periodicMessage);

            await _periodicMessageRepository.UpdatePeriodicMessageAsync(periodicMessage);
            return _mapper.Map<PeriodicMessageDto>(periodicMessage);
        }

        /// <summary>
        /// Periyodik mesaj siler
        /// </summary>
        public async Task DeletePeriodicMessageAsync(int id)
        {
            var periodicMessage = await _periodicMessageRepository.GetPeriodicMessageByIdAsync(id);
            if (periodicMessage == null)
            {
                throw new Exception("Periyodik mesaj bulunamadı.");
            }

            await _periodicMessageRepository.DeletePeriodicMessageAsync(id);
        }
    }
} 