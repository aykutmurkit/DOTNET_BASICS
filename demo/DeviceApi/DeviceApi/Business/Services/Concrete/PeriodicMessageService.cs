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

        public PeriodicMessageService(
            IPeriodicMessageRepository periodicMessageRepository,
            IDeviceRepository deviceRepository)
        {
            _periodicMessageRepository = periodicMessageRepository;
            _deviceRepository = deviceRepository;
        }

        /// <summary>
        /// Tüm periyodik mesajları getirir
        /// </summary>
        public async Task<List<PeriodicMessageDto>> GetAllPeriodicMessagesAsync()
        {
            var messages = await _periodicMessageRepository.GetAllPeriodicMessagesAsync();
            return messages.Select(m => MapToDto(m)).ToList();
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

            return MapToDto(message);
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

            return MapToDto(message);
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

            var periodicMessage = new PeriodicMessage
            {
                TemperatureLevel = request.TemperatureLevel,
                HumidityLevel = request.HumidityLevel,
                GasLevel = request.GasLevel,
                FrontLightLevel = request.FrontLightLevel,
                BackLightLevel = request.BackLightLevel,
                LedFailureCount = request.LedFailureCount,
                CabinStatus = request.CabinStatus,
                FanStatus = request.FanStatus,
                ShowStatus = request.ShowStatus,
                Rs232Status = request.Rs232Status,
                PowerSupplyStatus = request.PowerSupplyStatus,
                DeviceId = request.DeviceId,
                CreatedAt = DateTime.Now
            };

            await _periodicMessageRepository.AddPeriodicMessageAsync(periodicMessage);
            return MapToDto(periodicMessage);
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

            periodicMessage.TemperatureLevel = request.TemperatureLevel;
            periodicMessage.HumidityLevel = request.HumidityLevel;
            periodicMessage.GasLevel = request.GasLevel;
            periodicMessage.FrontLightLevel = request.FrontLightLevel;
            periodicMessage.BackLightLevel = request.BackLightLevel;
            periodicMessage.LedFailureCount = request.LedFailureCount;
            periodicMessage.CabinStatus = request.CabinStatus;
            periodicMessage.FanStatus = request.FanStatus;
            periodicMessage.ShowStatus = request.ShowStatus;
            periodicMessage.Rs232Status = request.Rs232Status;
            periodicMessage.PowerSupplyStatus = request.PowerSupplyStatus;
            periodicMessage.ForecastedAt = DateTime.Now;

            await _periodicMessageRepository.UpdatePeriodicMessageAsync(periodicMessage);
            return MapToDto(periodicMessage);
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

        /// <summary>
        /// PeriodicMessage entity'sini PeriodicMessageDto'ya dönüştürür
        /// </summary>
        private PeriodicMessageDto MapToDto(PeriodicMessage periodicMessage)
        {
            return new PeriodicMessageDto
            {
                Id = periodicMessage.Id,
                TemperatureLevel = periodicMessage.TemperatureLevel,
                HumidityLevel = periodicMessage.HumidityLevel,
                GasLevel = periodicMessage.GasLevel,
                FrontLightLevel = periodicMessage.FrontLightLevel,
                BackLightLevel = periodicMessage.BackLightLevel,
                LedFailureCount = periodicMessage.LedFailureCount,
                CabinStatus = periodicMessage.CabinStatus,
                FanStatus = periodicMessage.FanStatus,
                ShowStatus = periodicMessage.ShowStatus,
                Rs232Status = periodicMessage.Rs232Status,
                PowerSupplyStatus = periodicMessage.PowerSupplyStatus,
                CreatedAt = periodicMessage.CreatedAt,
                ForecastedAt = periodicMessage.ForecastedAt,
                DeviceId = periodicMessage.DeviceId
            };
        }
    }
} 