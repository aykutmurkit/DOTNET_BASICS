using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Kayan ekran mesaj repository implementasyonu
    /// </summary>
    public class ScrollingScreenMessageRepository : IScrollingScreenMessageRepository
    {
        private readonly AppDbContext _context;

        public ScrollingScreenMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScrollingScreenMessage>> GetAllScrollingScreenMessagesAsync()
        {
            return await _context.ScrollingScreenMessages
                .Include(m => m.Device)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ScrollingScreenMessage> GetScrollingScreenMessageByIdAsync(int id)
        {
            return await _context.ScrollingScreenMessages
                .Include(m => m.Device)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<ScrollingScreenMessage> GetScrollingScreenMessageByDeviceIdAsync(int deviceId)
        {
            return await _context.ScrollingScreenMessages
                .Include(m => m.Device)
                .FirstOrDefaultAsync(m => m.DeviceId == deviceId);
        }

        public async Task AddScrollingScreenMessageAsync(ScrollingScreenMessage scrollingScreenMessage)
        {
            scrollingScreenMessage.CreatedAt = DateTime.Now;
            await _context.ScrollingScreenMessages.AddAsync(scrollingScreenMessage);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateScrollingScreenMessageAsync(ScrollingScreenMessage scrollingScreenMessage)
        {
            scrollingScreenMessage.UpdatedAt = DateTime.Now;
            _context.ScrollingScreenMessages.Update(scrollingScreenMessage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteScrollingScreenMessageAsync(int id)
        {
            var scrollingScreenMessage = await _context.ScrollingScreenMessages.FindAsync(id);
            if (scrollingScreenMessage != null)
            {
                _context.ScrollingScreenMessages.Remove(scrollingScreenMessage);
                await _context.SaveChangesAsync();
            }
        }
    }
} 