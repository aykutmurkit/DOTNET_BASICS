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

        public PredictionService(
            IPredictionRepository predictionRepository,
            IPlatformRepository platformRepository,
            ILogService logService)
        {
            _predictionRepository = predictionRepository;
            _platformRepository = platformRepository;
            _logService = logService;
        }

        /// <summary>
        /// Tüm tren tahminlerini getirir
        /// </summary>
        public async Task<List<PredictionDto>> GetAllPredictionsAsync()
        {
            var predictions = await _predictionRepository.GetAllPredictionsAsync();
            return predictions.Select(MapToPredictionDto).ToList();
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
            
            return MapToPredictionDto(prediction);
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
            
            return MapToPredictionDto(prediction);
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
            var prediction = new Prediction
            {
                StationName = request.StationName,
                Direction = request.Direction,
                Train1 = request.Train1,
                Line1 = request.Line1,
                Destination1 = request.Destination1,
                Time1 = request.Time1,
                Train2 = request.Train2,
                Line2 = request.Line2,
                Destination2 = request.Destination2,
                Time2 = request.Time2,
                Train3 = request.Train3,
                Line3 = request.Line3,
                Destination3 = request.Destination3,
                Time3 = request.Time3,
                ForecastGenerationAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                PlatformId = platformId
            };
            
            await _predictionRepository.AddPredictionAsync(prediction);
            
            await _logService.LogInfoAsync(
                "Tren tahmini oluşturuldu", 
                "PredictionService.CreatePrediction", 
                new { PredictionId = prediction.Id, PlatformId = platformId });
            
            return MapToPredictionDto(prediction);
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
            existingPrediction.StationName = request.StationName;
            existingPrediction.Direction = request.Direction;
            existingPrediction.Train1 = request.Train1;
            existingPrediction.Line1 = request.Line1;
            existingPrediction.Destination1 = request.Destination1;
            existingPrediction.Time1 = request.Time1;
            existingPrediction.Train2 = request.Train2;
            existingPrediction.Line2 = request.Line2;
            existingPrediction.Destination2 = request.Destination2;
            existingPrediction.Time2 = request.Time2;
            existingPrediction.Train3 = request.Train3;
            existingPrediction.Line3 = request.Line3;
            existingPrediction.Destination3 = request.Destination3;
            existingPrediction.Time3 = request.Time3;
            existingPrediction.ForecastGenerationAt = DateTime.Now;
            
            await _predictionRepository.UpdatePredictionAsync(existingPrediction);
            
            await _logService.LogInfoAsync(
                "Tren tahmini güncellendi", 
                "PredictionService.UpdatePrediction", 
                new { PredictionId = id });
            
            return MapToPredictionDto(existingPrediction);
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

        /// <summary>
        /// Entity'den DTO'ya dönüşüm
        /// </summary>
        private PredictionDto MapToPredictionDto(Prediction prediction)
        {
            return new PredictionDto
            {
                Id = prediction.Id,
                StationName = prediction.StationName,
                Direction = prediction.Direction,
                Train1 = prediction.Train1,
                Line1 = prediction.Line1,
                Destination1 = prediction.Destination1,
                Time1 = prediction.Time1,
                Train2 = prediction.Train2,
                Line2 = prediction.Line2,
                Destination2 = prediction.Destination2,
                Time2 = prediction.Time2,
                Train3 = prediction.Train3,
                Line3 = prediction.Line3,
                Destination3 = prediction.Destination3,
                Time3 = prediction.Time3,
                ForecastGenerationAt = prediction.ForecastGenerationAt,
                PlatformId = prediction.PlatformId,
                CreatedAt = prediction.CreatedAt
            };
        }
    }
} 