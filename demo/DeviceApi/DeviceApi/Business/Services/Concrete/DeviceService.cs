using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Cihaz servisi implementasyonu
    /// </summary>
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IPlatformRepository _platformRepository;
        private readonly IStationRepository _stationRepository;

        public DeviceService(IDeviceRepository deviceRepository, IPlatformRepository platformRepository, IStationRepository stationRepository)
        {
            _deviceRepository = deviceRepository;
            _platformRepository = platformRepository;
            _stationRepository = stationRepository;
        }

        public async Task<List<DeviceDto>> GetAllDevicesAsync()
        {
            var devices = await _deviceRepository.GetAllDevicesAsync();
            return devices.Select(MapToDeviceDto).ToList();
        }

        public async Task<DeviceDto> GetDeviceByIdAsync(int id)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(id);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            return MapToDeviceDto(device);
        }
        
        public async Task<List<DeviceDto>> GetDevicesByPlatformIdAsync(int platformId)
        {
            // Platform var mı kontrol et
            var platform = await _platformRepository.GetPlatformByIdAsync(platformId);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı.");
            }
            
            var devices = await _deviceRepository.GetDevicesByPlatformIdAsync(platformId);
            return devices.Select(MapToDeviceDto).ToList();
        }
        
        public async Task<List<DeviceDto>> GetDevicesByStationIdAsync(int stationId)
        {
            // İstasyon var mı kontrol et
            var station = await _stationRepository.GetStationByIdAsync(stationId);
            if (station == null)
            {
                throw new Exception("İstasyon bulunamadı.");
            }
            
            var devices = await _deviceRepository.GetDevicesByStationIdAsync(stationId);
            return devices.Select(MapToDeviceDto).ToList();
        }

        public async Task<DeviceDto> CreateDeviceAsync(CreateDeviceRequest request)
        {
            // Platform var mı kontrol et
            var platform = await _platformRepository.GetPlatformByIdAsync(request.PlatformId);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı.");
            }
            
            // IP ve port kombinasyonu zaten kullanılıyor mu kontrol et
            if (await _deviceRepository.IpPortCombinationExistsAsync(request.Ip, request.Port))
            {
                throw new Exception($"Bu IP adresi ({request.Ip}) ve port ({request.Port}) kombinasyonu zaten kullanılıyor.");
            }

            var device = new Device
            {
                Name = request.Name,
                Ip = request.Ip,
                Port = request.Port,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                PlatformId = request.PlatformId
            };

            await _deviceRepository.AddDeviceAsync(device);
            
            // İlişkileri içeren device nesnesini çek
            var createdDevice = await _deviceRepository.GetDeviceByIdAsync(device.Id);
            return MapToDeviceDto(createdDevice);
        }

        public async Task<DeviceDto> UpdateDeviceAsync(int id, UpdateDeviceRequest request)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(id);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            // Platform var mı kontrol et
            var platform = await _platformRepository.GetPlatformByIdAsync(request.PlatformId);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı.");
            }
            
            // IP ve port kombinasyonu zaten kullanılıyor mu kontrol et (cihazın kendisi hariç)
            if (await _deviceRepository.IpPortCombinationExistsAsync(request.Ip, request.Port, id))
            {
                throw new Exception($"Bu IP adresi ({request.Ip}) ve port ({request.Port}) kombinasyonu zaten kullanılıyor.");
            }

            device.Name = request.Name;
            device.Ip = request.Ip;
            device.Port = request.Port;
            device.Latitude = request.Latitude;
            device.Longitude = request.Longitude;
            device.PlatformId = request.PlatformId;

            await _deviceRepository.UpdateDeviceAsync(device);
            
            // İlişkileri içeren device nesnesini çek
            var updatedDevice = await _deviceRepository.GetDeviceByIdAsync(id);
            return MapToDeviceDto(updatedDevice);
        }

        public async Task DeleteDeviceAsync(int id)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(id);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            await _deviceRepository.DeleteDeviceAsync(id);
        }

        private DeviceDto MapToDeviceDto(Device device)
        {
            string stationName = "Bilinmiyor";
            
            if (device.Platform?.Station != null)
            {
                stationName = device.Platform.Station.Name;
            }
            
            return new DeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                Ip = device.Ip,
                Port = device.Port,
                Latitude = device.Latitude,
                Longitude = device.Longitude,
                PlatformId = device.PlatformId,
                PlatformStationName = stationName
            };
        }
    }
} 