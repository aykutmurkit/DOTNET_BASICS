using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Periyodik mesaj repository implementasyonu
    /// </summary>
    public class PeriodicMessageRepository : IPeriodicMessageRepository
    {
        private readonly AppDbContext _context;

        public PeriodicMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PeriodicMessage>> GetAllPeriodicMessagesAsync()
        {
            return await _context.PeriodicMessages
                .Include(m => m.Device)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PeriodicMessage> GetPeriodicMessageByIdAsync(int id)
        {
            return await _context.PeriodicMessages
                .Include(m => m.Device)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PeriodicMessage> GetPeriodicMessageByDeviceIdAsync(int deviceId)
        {
            return await _context.PeriodicMessages
                .Include(m => m.Device)
                .FirstOrDefaultAsync(m => m.DeviceId == deviceId);
        }

        public async Task AddPeriodicMessageAsync(PeriodicMessage periodicMessage)
        {
            periodicMessage.CreatedAt = DateTime.Now;
            await _context.PeriodicMessages.AddAsync(periodicMessage);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePeriodicMessageAsync(PeriodicMessage periodicMessage)
        {
            periodicMessage.UpdatedAt = DateTime.Now;
            _context.PeriodicMessages.Update(periodicMessage);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePeriodicMessageAsync(int id)
        {
            var periodicMessage = await _context.PeriodicMessages.FindAsync(id);
            if (periodicMessage != null)
            {
                _context.PeriodicMessages.Remove(periodicMessage);
                await _context.SaveChangesAsync();
            }
        }
    }
} 