using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Tam ekran mesaj repository implementasyonu
    /// </summary>
    public class FullScreenMessageRepository : IFullScreenMessageRepository
    {
        private readonly AppDbContext _context;

        public FullScreenMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FullScreenMessage>> GetAllFullScreenMessagesAsync()
        {
            return await _context.FullScreenMessages
                .Include(f => f.Device)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FullScreenMessage> GetFullScreenMessageByIdAsync(int id)
        {
            return await _context.FullScreenMessages
                .Include(f => f.Device)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FullScreenMessage> GetFullScreenMessageByDeviceIdAsync(int deviceId)
        {
            return await _context.FullScreenMessages
                .Include(f => f.Device)
                .FirstOrDefaultAsync(f => f.DeviceId == deviceId);
        }

        public async Task AddFullScreenMessageAsync(FullScreenMessage fullScreenMessage)
        {
            fullScreenMessage.CreatedAt = DateTime.Now;
            await _context.FullScreenMessages.AddAsync(fullScreenMessage);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFullScreenMessageAsync(FullScreenMessage fullScreenMessage)
        {
            fullScreenMessage.ModifiedAt = DateTime.Now;
            _context.FullScreenMessages.Update(fullScreenMessage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFullScreenMessageAsync(int id)
        {
            var fullScreenMessage = await _context.FullScreenMessages.FindAsync(id);
            if (fullScreenMessage != null)
            {
                _context.FullScreenMessages.Remove(fullScreenMessage);
                await _context.SaveChangesAsync();
            }
        }
    }
} 