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
                .Include(m => m.Devices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<BitmapScreenMessage> GetBitmapScreenMessageByIdAsync(int id)
        {
            return await _context.BitmapScreenMessages
                .Include(m => m.Devices)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<BitmapScreenMessage> GetBitmapScreenMessageByDeviceIdAsync(int deviceId)
        {
            // Önce cihazı bul, sonra cihazın bağlı olduğu mesajı getir
            var device = await _context.Devices
                .Include(d => d.BitmapScreenMessage)
                .FirstOrDefaultAsync(d => d.Id == deviceId);
            
            return device?.BitmapScreenMessage;
        }
        
        public async Task<List<Device>> GetDevicesByBitmapScreenMessageIdAsync(int bitmapScreenMessageId)
        {
            return await _context.Devices
                .Where(d => d.BitmapScreenMessageId == bitmapScreenMessageId)
                .ToListAsync();
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
                // Önce ilişkili cihazların bağlantısını kaldır
                var devices = await _context.Devices
                    .Where(d => d.BitmapScreenMessageId == id)
                    .ToListAsync();
                
                foreach (var device in devices)
                {
                    device.BitmapScreenMessageId = null;
                }
                
                _context.BitmapScreenMessages.Remove(bitmapScreenMessage);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task AssignMessageToDeviceAsync(int deviceId, int bitmapScreenMessageId)
        {
            // Cihazı bul
            var device = await _context.Devices.FindAsync(deviceId);
            if (device != null)
            {
                // Bitmap mesajının var olup olmadığını kontrol et
                var bitmapMessage = await _context.BitmapScreenMessages.FindAsync(bitmapScreenMessageId);
                if (bitmapMessage != null)
                {
                    // Cihaza mesajı ata
                    device.BitmapScreenMessageId = bitmapScreenMessageId;
                    await _context.SaveChangesAsync();
                }
            }
        }
        
        public async Task UnassignMessageFromDeviceAsync(int deviceId)
        {
            // Cihazı bul
            var device = await _context.Devices.FindAsync(deviceId);
            if (device != null)
            {
                // Cihazdan mesaj bağlantısını kaldır
                device.BitmapScreenMessageId = null;
                await _context.SaveChangesAsync();
            }
        }
    }
} 