using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Tren tahminleri için repository arayüzü
    /// </summary>
    public interface IPredictionRepository
    {
        /// <summary>
        /// Tüm tahminleri getirir
        /// </summary>
        Task<List<Prediction>> GetAllPredictionsAsync();
        
        /// <summary>
        /// ID'ye göre tahmin getirir
        /// </summary>
        Task<Prediction> GetPredictionByIdAsync(int id);
        
        /// <summary>
        /// Platform ID'sine göre tahmin getirir (one-to-one ilişki)
        /// </summary>
        Task<Prediction> GetPredictionByPlatformIdAsync(int platformId);
        
        /// <summary>
        /// Yeni tahmin ekler
        /// </summary>
        Task AddPredictionAsync(Prediction prediction);
        
        /// <summary>
        /// Tahmin günceller
        /// </summary>
        Task UpdatePredictionAsync(Prediction prediction);
        
        /// <summary>
        /// Tahmin siler
        /// </summary>
        Task DeletePredictionAsync(int id);
    }
} 