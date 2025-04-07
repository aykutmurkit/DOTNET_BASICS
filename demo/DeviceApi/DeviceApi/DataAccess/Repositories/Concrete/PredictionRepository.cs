using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Tren tahminleri için repository implementasyonu
    /// </summary>
    public class PredictionRepository : IPredictionRepository
    {
        private readonly AppDbContext _context;

        public PredictionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Prediction>> GetAllPredictionsAsync()
        {
            return await _context.Predictions
                .Include(p => p.Platform)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Prediction> GetPredictionByIdAsync(int id)
        {
            return await _context.Predictions
                .Include(p => p.Platform)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Prediction> GetPredictionByPlatformIdAsync(int platformId)
        {
            return await _context.Predictions
                .Include(p => p.Platform)
                .FirstOrDefaultAsync(p => p.PlatformId == platformId);
        }

        public async Task AddPredictionAsync(Prediction prediction)
        {
            prediction.CreatedAt = DateTime.Now;
            prediction.ForecastGenerationAt = DateTime.Now;
            
            await _context.Predictions.AddAsync(prediction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePredictionAsync(Prediction prediction)
        {
            var existingPrediction = await _context.Predictions.FindAsync(prediction.Id);
            if (existingPrediction != null)
            {
                // CreatedAt değerini koruyalım
                prediction.CreatedAt = existingPrediction.CreatedAt;
                
                // ForecastGenerationAt değerini güncelle
                prediction.ForecastGenerationAt = DateTime.Now;
                
                _context.Entry(existingPrediction).State = EntityState.Detached;
                _context.Predictions.Update(prediction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePredictionAsync(int id)
        {
            var prediction = await _context.Predictions.FindAsync(id);
            if (prediction != null)
            {
                _context.Predictions.Remove(prediction);
                await _context.SaveChangesAsync();
            }
        }
    }
} 