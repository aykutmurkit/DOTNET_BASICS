using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Cihaz durum servisi implementasyonu
    /// </summary>
    public class DeviceStatusService : IDeviceStatusService
    {
        private readonly IDeviceStatusRepository _deviceStatusRepository;
        private readonly IDeviceRepository _deviceRepository;

        public DeviceStatusService(IDeviceStatusRepository deviceStatusRepository, IDeviceRepository deviceRepository)
        {
            _deviceStatusRepository = deviceStatusRepository;
            _deviceRepository = deviceRepository;
        }

        public async Task<List<DeviceStatusDto>> GetAllDeviceStatusesAsync()
        {
            var deviceStatuses = await _deviceStatusRepository.GetAllDeviceStatusesAsync();
            return deviceStatuses.Select(MapToDeviceStatusDto).ToList();
        }

        public async Task<DeviceStatusDto> GetDeviceStatusByIdAsync(int id)
        {
            var deviceStatus = await _deviceStatusRepository.GetDeviceStatusByIdAsync(id);
            if (deviceStatus == null)
            {
                throw new Exception("Cihaz durumu bulunamadı.");
            }

            return MapToDeviceStatusDto(deviceStatus);
        }
        
        public async Task<DeviceStatusDto> GetDeviceStatusByDeviceIdAsync(int deviceId)
        {
            var deviceStatus = await _deviceStatusRepository.GetDeviceStatusByDeviceIdAsync(deviceId);
            if (deviceStatus == null)
            {
                throw new Exception("Cihaz durumu bulunamadı.");
            }

            return MapToDeviceStatusDto(deviceStatus);
        }

        public async Task<DeviceStatusDto> AddDeviceStatusAsync(CreateDeviceStatusDto createDeviceStatusDto)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(createDeviceStatusDto.DeviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            var existingStatus = await _deviceStatusRepository.GetDeviceStatusByDeviceIdAsync(createDeviceStatusDto.DeviceId);
            if (existingStatus != null)
            {
                throw new Exception("Bu cihaza ait durum bilgisi zaten mevcut.");
            }

            var deviceStatus = new DeviceStatus
            {
                FullScreenMessageStatus = createDeviceStatusDto.FullScreenMessageStatus,
                ScrollingScreenMessageStatus = createDeviceStatusDto.ScrollingScreenMessageStatus,
                BitmapScreenMessageStatus = createDeviceStatusDto.BitmapScreenMessageStatus,
                DeviceId = createDeviceStatusDto.DeviceId,
                CreatedAt = DateTime.Now
            };

            await _deviceStatusRepository.AddDeviceStatusAsync(deviceStatus);
            return MapToDeviceStatusDto(deviceStatus);
        }

        public async Task<DeviceStatusDto> UpdateDeviceStatusAsync(int id, UpdateDeviceStatusDto updateDeviceStatusDto)
        {
            var deviceStatus = await _deviceStatusRepository.GetDeviceStatusByIdAsync(id);
            if (deviceStatus == null)
            {
                throw new Exception("Cihaz durumu bulunamadı.");
            }

            var device = await _deviceRepository.GetDeviceByIdAsync(updateDeviceStatusDto.DeviceId);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            deviceStatus.FullScreenMessageStatus = updateDeviceStatusDto.FullScreenMessageStatus;
            deviceStatus.ScrollingScreenMessageStatus = updateDeviceStatusDto.ScrollingScreenMessageStatus;
            deviceStatus.BitmapScreenMessageStatus = updateDeviceStatusDto.BitmapScreenMessageStatus;
            deviceStatus.DeviceId = updateDeviceStatusDto.DeviceId;
            deviceStatus.UpdatedAt = DateTime.Now;

            await _deviceStatusRepository.UpdateDeviceStatusAsync(deviceStatus);
            return MapToDeviceStatusDto(deviceStatus);
        }

        public async Task DeleteDeviceStatusAsync(int id)
        {
            var deviceStatus = await _deviceStatusRepository.GetDeviceStatusByIdAsync(id);
            if (deviceStatus == null)
            {
                throw new Exception("Cihaz durumu bulunamadı.");
            }

            await _deviceStatusRepository.DeleteDeviceStatusAsync(id);
        }

        private DeviceStatusDto MapToDeviceStatusDto(DeviceStatus deviceStatus)
        {
            return new DeviceStatusDto
            {
                Id = deviceStatus.Id,
                FullScreenMessageStatus = deviceStatus.FullScreenMessageStatus,
                ScrollingScreenMessageStatus = deviceStatus.ScrollingScreenMessageStatus,
                BitmapScreenMessageStatus = deviceStatus.BitmapScreenMessageStatus,
                DeviceId = deviceStatus.DeviceId,
                DeviceName = deviceStatus.Device?.Name,
                CreatedAt = deviceStatus.CreatedAt,
                UpdatedAt = deviceStatus.UpdatedAt
            };
        }
    }
} 