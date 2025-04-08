using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Tahmin servis arayüzü
    /// </summary>
    public interface IPredictionService
    {
        /// <summary>
        /// Tüm tren tahminlerini getirir
        /// </summary>
        Task<List<PredictionDto>> GetAllPredictionsAsync();

        /// <summary>
        /// ID'ye göre tren tahmini getirir
        /// </summary>
        Task<PredictionDto> GetPredictionByIdAsync(int id);
        
        /// <summary>
        /// Platform ID'sine göre tren tahmini getirir
        /// </summary>
        Task<PredictionDto> GetPredictionByPlatformIdAsync(int platformId);

        /// <summary>
        /// Platform için yeni bir tren tahmini oluşturur
        /// </summary>
        Task<PredictionDto> CreatePredictionAsync(int platformId, CreatePredictionRequest request);

        /// <summary>
        /// Tren tahmini günceller
        /// </summary>
        Task<PredictionDto> UpdatePredictionAsync(int id, UpdatePredictionRequest request);

        /// <summary>
        /// Tren tahmini siler
        /// </summary>
        Task DeletePredictionAsync(int id);
    }
} 