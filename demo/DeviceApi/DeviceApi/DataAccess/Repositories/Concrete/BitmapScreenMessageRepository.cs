using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Bitmap ekran mesaj repository implementasyonu
    /// </summary>
    public class BitmapScreenMessageRepository : IBitmapScreenMessageRepository
    {
        private readonly AppDbContext _context;

        public BitmapScreenMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BitmapScreenMessage>> GetAllBitmapScreenMessagesAsync()
        {
            return await _context.BitmapScreenMessages
                .Include(m => m.Device)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<BitmapScreenMessage> GetBitmapScreenMessageByIdAsync(int id)
        {
            return await _context.BitmapScreenMessages
                .Include(m => m.Device)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<BitmapScreenMessage> GetBitmapScreenMessageByDeviceIdAsync(int deviceId)
        {
            return await _context.BitmapScreenMessages
                .Include(m => m.Device)
                .FirstOrDefaultAsync(m => m.DeviceId == deviceId);
        }

        public async Task AddBitmapScreenMessageAsync(BitmapScreenMessage bitmapScreenMessage)
        {
            bitmapScreenMessage.CreatedAt = DateTime.Now;
            await _context.BitmapScreenMessages.AddAsync(bitmapScreenMessage);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBitmapScreenMessageAsync(BitmapScreenMessage bitmapScreenMessage)
        {
            bitmapScreenMessage.UpdatedAt = DateTime.Now;
            _context.BitmapScreenMessages.Update(bitmapScreenMessage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBitmapScreenMessageAsync(int id)
        {
            var bitmapScreenMessage = await _context.BitmapScreenMessages.FindAsync(id);
            if (bitmapScreenMessage != null)
            {
                _context.BitmapScreenMessages.Remove(bitmapScreenMessage);
                await _context.SaveChangesAsync();
            }
        }
    }
} 