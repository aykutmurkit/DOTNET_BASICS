using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// İstasyon servisi implementasyonu
    /// </summary>
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;

        public StationService(IStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
        }

        public async Task<List<StationDto>> GetAllStationsAsync()
        {
            var stations = await _stationRepository.GetAllStationsWithRelationsAsync();
            return stations.Select(MapToStationDto).ToList();
        }

        public async Task<StationDto> GetStationByIdAsync(int id)
        {
            var station = await _stationRepository.GetStationByIdWithRelationsAsync(id);
            if (station == null)
            {
                throw new Exception("İstasyon bulunamadı.");
            }

            return MapToStationDto(station);
        }

        public async Task<StationDto> CreateStationAsync(CreateStationRequest request)
        {
            // İstasyon adının benzersiz olduğunu kontrol et
            if (await _stationRepository.StationNameExistsAsync(request.Name))
            {
                throw new Exception("Bu isimde bir istasyon zaten mevcut.");
            }

            var station = new Station
            {
                Name = request.Name,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            await _stationRepository.AddStationAsync(station);
            
            // İlişkileri içeren station nesnesini çek
            var createdStation = await _stationRepository.GetStationByIdWithRelationsAsync(station.Id);
            return MapToStationDto(createdStation);
        }

        public async Task<StationDto> UpdateStationAsync(int id, UpdateStationRequest request)
        {
            var station = await _stationRepository.GetStationByIdAsync(id);
            if (station == null)
            {
                throw new Exception("İstasyon bulunamadı.");
            }

            // İstasyon adının benzersiz olduğunu kontrol et
            if (station.Name != request.Name && await _stationRepository.StationNameExistsAsync(request.Name))
            {
                throw new Exception("Bu isimde bir istasyon zaten mevcut.");
            }

            station.Name = request.Name;
            station.Latitude = request.Latitude;
            station.Longitude = request.Longitude;

            await _stationRepository.UpdateStationAsync(station);
            
            // İlişkileri içeren station nesnesini çek
            var updatedStation = await _stationRepository.GetStationByIdWithRelationsAsync(id);
            return MapToStationDto(updatedStation);
        }

        public async Task DeleteStationAsync(int id)
        {
            var station = await _stationRepository.GetStationByIdAsync(id);
            if (station == null)
            {
                throw new Exception("İstasyon bulunamadı.");
            }

            await _stationRepository.DeleteStationAsync(id);
        }

        private StationDto MapToStationDto(Station station)
        {
            return new StationDto
            {
                Id = station.Id,
                Name = station.Name,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                Platforms = station.Platforms?.Select(p => new PlatformDto
                {
                    Id = p.Id,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    StationId = p.StationId,
                    StationName = station.Name,
                    Devices = p.Devices?.Select(d => new DeviceDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Ip = d.Ip,
                        Port = d.Port,
                        Latitude = d.Latitude,
                        Longitude = d.Longitude,
                        PlatformId = d.PlatformId,
                        PlatformStationName = station.Name,
                        Status = d.Status != null ? new DeviceStatusDto
                        {
                            Id = d.Status.Id,
                            FullScreenMessageStatus = d.Status.FullScreenMessageStatus,
                            ScrollingScreenMessageStatus = d.Status.ScrollingScreenMessageStatus,
                            BitmapScreenMessageStatus = d.Status.BitmapScreenMessageStatus,
                            DeviceId = d.Status.DeviceId,
                            DeviceName = d.Name,
                            CreatedAt = d.Status.CreatedAt,
                            UpdatedAt = d.Status.UpdatedAt
                        } : null
                    }).ToList() ?? new List<DeviceDto>()
                }).ToList() ?? new List<PlatformDto>()
            };
        }
    }
} 