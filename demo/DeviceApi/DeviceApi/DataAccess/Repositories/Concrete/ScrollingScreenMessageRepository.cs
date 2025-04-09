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
                .Include(m => m.Devices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ScrollingScreenMessage> GetScrollingScreenMessageByIdAsync(int id)
        {
            return await _context.ScrollingScreenMessages
                .Include(m => m.Devices)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<ScrollingScreenMessage> GetScrollingScreenMessageByDeviceIdAsync(int deviceId)
        {
            // Önce cihazı bul, sonra cihazın bağlı olduğu mesajı getir
            var device = await _context.Devices
                .Include(d => d.ScrollingScreenMessage)
                .FirstOrDefaultAsync(d => d.Id == deviceId);
            
            return device?.ScrollingScreenMessage;
        }
        
        public async Task<List<Device>> GetDevicesByScrollingScreenMessageIdAsync(int scrollingScreenMessageId)
        {
            return await _context.Devices
                .Where(d => d.ScrollingScreenMessageId == scrollingScreenMessageId)
                .ToListAsync();
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
                // Önce ilişkili cihazların bağlantısını kaldır
                var devices = await _context.Devices
                    .Where(d => d.ScrollingScreenMessageId == id)
                    .ToListAsync();
                
                foreach (var device in devices)
                {
                    device.ScrollingScreenMessageId = null;
                }
                
                _context.ScrollingScreenMessages.Remove(scrollingScreenMessage);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task AssignMessageToDeviceAsync(int deviceId, int scrollingScreenMessageId)
        {
            // Cihaz ve mesajın var olduğunu kontrol et
            var device = await _context.Devices.FindAsync(deviceId);
            var message = await _context.ScrollingScreenMessages.FindAsync(scrollingScreenMessageId);
            
            if (device != null && message != null)
            {
                device.ScrollingScreenMessageId = scrollingScreenMessageId;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Cihaz veya mesaj bulunamadı");
            }
        }
        
        public async Task UnassignMessageFromDeviceAsync(int deviceId)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            
            if (device != null)
            {
                device.ScrollingScreenMessageId = null;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Cihaz bulunamadı");
            }
        }
    }
} 