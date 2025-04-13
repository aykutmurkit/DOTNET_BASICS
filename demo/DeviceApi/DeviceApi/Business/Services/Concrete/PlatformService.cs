using AutoMapper;
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
        private readonly IMapper _mapper;

        public PlatformService(
            IPlatformRepository platformRepository, 
            IStationRepository stationRepository,
            IMapper mapper)
        {
            _platformRepository = platformRepository;
            _stationRepository = stationRepository;
            _mapper = mapper;
        }

        public async Task<List<PlatformDto>> GetAllPlatformsAsync()
        {
            var platforms = await _platformRepository.GetAllPlatformsWithDevicesAsync();
            return _mapper.Map<List<PlatformDto>>(platforms);
        }

        public async Task<PlatformDto> GetPlatformByIdAsync(int id)
        {
            var platform = await _platformRepository.GetPlatformByIdWithDevicesAsync(id);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı.");
            }

            return _mapper.Map<PlatformDto>(platform);
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
            return _mapper.Map<List<PlatformDto>>(platforms);
        }

        public async Task<PlatformDto> CreatePlatformAsync(CreatePlatformRequest request)
        {
            // İstasyon var mı kontrol et
            var station = await _stationRepository.GetStationByIdAsync(request.StationId);
            if (station == null)
            {
                throw new Exception("İstasyon bulunamadı.");
            }

            var platform = _mapper.Map<Platform>(request);

            await _platformRepository.AddPlatformAsync(platform);
            
            // İlişkileri içeren platform nesnesini çek
            var createdPlatform = await _platformRepository.GetPlatformByIdWithDevicesAsync(platform.Id);
            return _mapper.Map<PlatformDto>(createdPlatform);
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

            _mapper.Map(request, platform);

            await _platformRepository.UpdatePlatformAsync(platform);
            
            // İlişkileri içeren platform nesnesini çek
            var updatedPlatform = await _platformRepository.GetPlatformByIdWithDevicesAsync(id);
            return _mapper.Map<PlatformDto>(updatedPlatform);
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
    }
} 