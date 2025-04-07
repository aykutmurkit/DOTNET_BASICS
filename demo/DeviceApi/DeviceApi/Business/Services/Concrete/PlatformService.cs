using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Platform servisi implementasyonu
    /// </summary>
    public class PlatformService : IPlatformService
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IStationRepository _stationRepository;

        public PlatformService(IPlatformRepository platformRepository, IStationRepository stationRepository)
        {
            _platformRepository = platformRepository;
            _stationRepository = stationRepository;
        }

        public async Task<List<PlatformDto>> GetAllPlatformsAsync()
        {
            var platforms = await _platformRepository.GetAllPlatformsWithDevicesAsync();
            return platforms.Select(MapToPlatformDto).ToList();
        }

        public async Task<PlatformDto> GetPlatformByIdAsync(int id)
        {
            var platform = await _platformRepository.GetPlatformByIdWithDevicesAsync(id);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı.");
            }

            return MapToPlatformDto(platform);
        }
        
        public async Task<List<PlatformDto>> GetPlatformsByStationIdAsync(int stationId)
        {
            // İstasyon var mı kontrol et
            var station = await _stationRepository.GetStationByIdAsync(stationId);
            if (station == null)
            {
                throw new Exception("İstasyon bulunamadı.");
            }
            
            var platforms = await _platformRepository.GetPlatformsByStationIdAsync(stationId);
            return platforms.Select(MapToPlatformDto).ToList();
        }

        public async Task<PlatformDto> CreatePlatformAsync(CreatePlatformRequest request)
        {
            // İstasyon var mı kontrol et
            var station = await _stationRepository.GetStationByIdAsync(request.StationId);
            if (station == null)
            {
                throw new Exception("İstasyon bulunamadı.");
            }

            var platform = new Platform
            {
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                StationId = request.StationId
            };

            await _platformRepository.AddPlatformAsync(platform);
            
            // İlişkileri içeren platform nesnesini çek
            var createdPlatform = await _platformRepository.GetPlatformByIdWithDevicesAsync(platform.Id);
            return MapToPlatformDto(createdPlatform);
        }

        public async Task<PlatformDto> UpdatePlatformAsync(int id, UpdatePlatformRequest request)
        {
            var platform = await _platformRepository.GetPlatformByIdAsync(id);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı.");
            }

            // İstasyon var mı kontrol et
            var station = await _stationRepository.GetStationByIdAsync(request.StationId);
            if (station == null)
            {
                throw new Exception("İstasyon bulunamadı.");
            }

            platform.Latitude = request.Latitude;
            platform.Longitude = request.Longitude;
            platform.StationId = request.StationId;

            await _platformRepository.UpdatePlatformAsync(platform);
            
            // İlişkileri içeren platform nesnesini çek
            var updatedPlatform = await _platformRepository.GetPlatformByIdWithDevicesAsync(id);
            return MapToPlatformDto(updatedPlatform);
        }

        public async Task DeletePlatformAsync(int id)
        {
            var platform = await _platformRepository.GetPlatformByIdAsync(id);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı.");
            }

            await _platformRepository.DeletePlatformAsync(id);
        }

        private PlatformDto MapToPlatformDto(Platform platform)
        {
            return new PlatformDto
            {
                Id = platform.Id,
                Latitude = platform.Latitude,
                Longitude = platform.Longitude,
                StationId = platform.StationId,
                StationName = platform.Station?.Name ?? "Bilinmiyor",
                Devices = platform.Devices?.Select(d => new DeviceDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Ip = d.Ip,
                    Port = d.Port,
                    Latitude = d.Latitude,
                    Longitude = d.Longitude,
                    PlatformId = d.PlatformId,
                    PlatformStationName = platform.Station?.Name ?? "Bilinmiyor"
                }).ToList() ?? new List<DeviceDto>(),
                Prediction = platform.Prediction != null ? new PredictionDto
                {
                    Id = platform.Prediction.Id,
                    StationName = platform.Prediction.StationName,
                    Direction = platform.Prediction.Direction,
                    Train1 = platform.Prediction.Train1,
                    Line1 = platform.Prediction.Line1,
                    Destination1 = platform.Prediction.Destination1,
                    Time1 = platform.Prediction.Time1,
                    Train2 = platform.Prediction.Train2,
                    Line2 = platform.Prediction.Line2,
                    Destination2 = platform.Prediction.Destination2,
                    Time2 = platform.Prediction.Time2,
                    Train3 = platform.Prediction.Train3,
                    Line3 = platform.Prediction.Line3,
                    Destination3 = platform.Prediction.Destination3,
                    Time3 = platform.Prediction.Time3,
                    ForecastGenerationAt = platform.Prediction.ForecastGenerationAt,
                    CreatedAt = platform.Prediction.CreatedAt,
                    PlatformId = platform.Prediction.PlatformId
                } : null
            };
        }
    }
} 