using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using AutoMapper;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// İstasyon servisi implementasyonu
    /// </summary>
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;
        private readonly IMapper _mapper;

        public StationService(IStationRepository stationRepository, IMapper mapper)
        {
            _stationRepository = stationRepository;
            _mapper = mapper;
        }

        public async Task<List<StationDto>> GetAllStationsAsync()
        {
            var stations = await _stationRepository.GetAllStationsWithRelationsAsync();
            return _mapper.Map<List<StationDto>>(stations);
        }

        public async Task<StationDto> GetStationByIdAsync(int id)
        {
            var station = await _stationRepository.GetStationByIdWithRelationsAsync(id);
            if (station == null)
            {
                throw new Exception("İstasyon bulunamadı.");
            }

            return _mapper.Map<StationDto>(station);
        }

        public async Task<StationDto> CreateStationAsync(CreateStationRequest request)
        {
            // İstasyon adının benzersiz olduğunu kontrol et
            if (await _stationRepository.StationNameExistsAsync(request.Name))
            {
                throw new Exception("Bu isimde bir istasyon zaten mevcut.");
            }

            var station = _mapper.Map<Station>(request);

            await _stationRepository.AddStationAsync(station);
            
            // İlişkileri içeren station nesnesini çek
            var createdStation = await _stationRepository.GetStationByIdWithRelationsAsync(station.Id);
            return _mapper.Map<StationDto>(createdStation);
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

            _mapper.Map(request, station);

            await _stationRepository.UpdateStationAsync(station);
            
            // İlişkileri içeren station nesnesini çek
            var updatedStation = await _stationRepository.GetStationByIdWithRelationsAsync(id);
            return _mapper.Map<StationDto>(updatedStation);
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
    }
} 