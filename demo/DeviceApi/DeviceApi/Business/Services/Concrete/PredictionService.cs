using AutoMapper;
using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using LogLibrary.Core.Interfaces;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Tahmin servis implementasyonu
    /// </summary>
    public class PredictionService : IPredictionService
    {
        private readonly IPredictionRepository _predictionRepository;
        private readonly IPlatformRepository _platformRepository;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public PredictionService(
            IPredictionRepository predictionRepository,
            IPlatformRepository platformRepository,
            ILogService logService,
            IMapper mapper)
        {
            _predictionRepository = predictionRepository;
            _platformRepository = platformRepository;
            _logService = logService;
            _mapper = mapper;
        }

        /// <summary>
        /// Tüm tren tahminlerini getirir
        /// </summary>
        public async Task<List<PredictionDto>> GetAllPredictionsAsync()
        {
            var predictions = await _predictionRepository.GetAllPredictionsAsync();
            return _mapper.Map<List<PredictionDto>>(predictions);
        }

        /// <summary>
        /// ID'ye göre tren tahmini getirir
        /// </summary>
        public async Task<PredictionDto> GetPredictionByIdAsync(int id)
        {
            var prediction = await _predictionRepository.GetPredictionByIdAsync(id);
            if (prediction == null)
            {
                throw new Exception("Tren tahmini bulunamadı");
            }
            
            return _mapper.Map<PredictionDto>(prediction);
        }
        
        /// <summary>
        /// Platform ID'sine göre tren tahmini getirir
        /// </summary>
        public async Task<PredictionDto> GetPredictionByPlatformIdAsync(int platformId)
        {
            // Önce platformun var olduğunu kontrol et
            var platform = await _platformRepository.GetPlatformByIdAsync(platformId);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı");
            }
            
            var prediction = await _predictionRepository.GetPredictionByPlatformIdAsync(platformId);
            if (prediction == null)
            {
                throw new Exception("Platform için tren tahmini bulunamadı");
            }
            
            return _mapper.Map<PredictionDto>(prediction);
        }

        /// <summary>
        /// Platform için yeni bir tren tahmini oluşturur
        /// </summary>
        public async Task<PredictionDto> CreatePredictionAsync(int platformId, CreatePredictionRequest request)
        {
            // Platform var mı kontrol et
            var platform = await _platformRepository.GetPlatformByIdAsync(platformId);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı");
            }
            
            // Platform için zaten bir tahmin var mı kontrol et
            var existingPrediction = await _predictionRepository.GetPredictionByPlatformIdAsync(platformId);
            if (existingPrediction != null)
            {
                throw new Exception("Bu platform için zaten bir tren tahmini mevcut. Güncelleme yapabilirsiniz.");
            }
            
            // Yeni tahmin oluştur
            var prediction = _mapper.Map<Prediction>(request);
            prediction.PlatformId = platformId;
            
            await _predictionRepository.AddPredictionAsync(prediction);
            
            await _logService.LogInfoAsync(
                "Tren tahmini oluşturuldu", 
                "PredictionService.CreatePrediction", 
                new { PredictionId = prediction.Id, PlatformId = platformId });
            
            return _mapper.Map<PredictionDto>(prediction);
        }

        /// <summary>
        /// Tren tahmini günceller
        /// </summary>
        public async Task<PredictionDto> UpdatePredictionAsync(int id, UpdatePredictionRequest request)
        {
            // Tahmin var mı kontrol et
            var existingPrediction = await _predictionRepository.GetPredictionByIdAsync(id);
            if (existingPrediction == null)
            {
                throw new Exception("Güncellenecek tren tahmini bulunamadı");
            }
            
            // Tahmini güncelle
            _mapper.Map(request, existingPrediction);
            
            await _predictionRepository.UpdatePredictionAsync(existingPrediction);
            
            await _logService.LogInfoAsync(
                "Tren tahmini güncellendi", 
                "PredictionService.UpdatePrediction", 
                new { PredictionId = id });
            
            return _mapper.Map<PredictionDto>(existingPrediction);
        }

        /// <summary>
        /// Tren tahmini siler
        /// </summary>
        public async Task DeletePredictionAsync(int id)
        {
            // Tahmin var mı kontrol et
            var existingPrediction = await _predictionRepository.GetPredictionByIdAsync(id);
            if (existingPrediction == null)
            {
                throw new Exception("Silinecek tren tahmini bulunamadı");
            }
            
            await _predictionRepository.DeletePredictionAsync(id);
            
            await _logService.LogInfoAsync(
                "Tren tahmini silindi", 
                "PredictionService.DeletePrediction", 
                new { PredictionId = id });
        }
    }
} 