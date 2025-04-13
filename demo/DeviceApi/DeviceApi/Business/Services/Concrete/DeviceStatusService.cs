using AutoMapper;
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
        private readonly IMapper _mapper;

        public DeviceStatusService(
            IDeviceStatusRepository deviceStatusRepository, 
            IDeviceRepository deviceRepository,
            IMapper mapper)
        {
            _deviceStatusRepository = deviceStatusRepository;
            _deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        public async Task<List<DeviceStatusDto>> GetAllDeviceStatusesAsync()
        {
            var deviceStatuses = await _deviceStatusRepository.GetAllDeviceStatusesAsync();
            return _mapper.Map<List<DeviceStatusDto>>(deviceStatuses);
        }

        public async Task<DeviceStatusDto> GetDeviceStatusByIdAsync(int id)
        {
            var deviceStatus = await _deviceStatusRepository.GetDeviceStatusByIdAsync(id);
            if (deviceStatus == null)
            {
                throw new Exception("Cihaz durumu bulunamadı.");
            }

            return _mapper.Map<DeviceStatusDto>(deviceStatus);
        }
        
        public async Task<DeviceStatusDto> GetDeviceStatusByDeviceIdAsync(int deviceId)
        {
            var deviceStatus = await _deviceStatusRepository.GetDeviceStatusByDeviceIdAsync(deviceId);
            if (deviceStatus == null)
            {
                throw new Exception("Cihaz durumu bulunamadı.");
            }

            return _mapper.Map<DeviceStatusDto>(deviceStatus);
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

            var deviceStatus = _mapper.Map<DeviceStatus>(createDeviceStatusDto);

            await _deviceStatusRepository.AddDeviceStatusAsync(deviceStatus);
            return _mapper.Map<DeviceStatusDto>(deviceStatus);
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

            _mapper.Map(updateDeviceStatusDto, deviceStatus);

            await _deviceStatusRepository.UpdateDeviceStatusAsync(deviceStatus);
            return _mapper.Map<DeviceStatusDto>(deviceStatus);
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
    }
} 