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
                .Include(f => f.Devices)
                .Include(f => f.AlignmentType)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FullScreenMessage> GetFullScreenMessageByIdAsync(int id)
        {
            return await _context.FullScreenMessages
                .Include(f => f.Devices)
                .Include(f => f.AlignmentType)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FullScreenMessage> GetFullScreenMessageByDeviceIdAsync(int deviceId)
        {
            // Önce cihazı bul, sonra cihazın bağlı olduğu mesajı getir
            var device = await _context.Devices
                .Include(d => d.FullScreenMessage)
                .ThenInclude(f => f.AlignmentType)
                .FirstOrDefaultAsync(d => d.Id == deviceId);
            
            return device?.FullScreenMessage;
        }
        
        public async Task<List<Device>> GetDevicesByFullScreenMessageIdAsync(int fullScreenMessageId)
        {
            return await _context.Devices
                .Where(d => d.FullScreenMessageId == fullScreenMessageId)
                .ToListAsync();
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
                // Önce ilişkili cihazların bağlantısını kaldır
                var devices = await _context.Devices
                    .Where(d => d.FullScreenMessageId == id)
                    .ToListAsync();
                
                foreach (var device in devices)
                {
                    device.FullScreenMessageId = null;
                }
                
                _context.FullScreenMessages.Remove(fullScreenMessage);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task AssignMessageToDeviceAsync(int deviceId, int fullScreenMessageId)
        {
            // Cihaz ve mesajın var olduğunu kontrol et
            var device = await _context.Devices.FindAsync(deviceId);
            var message = await _context.FullScreenMessages.FindAsync(fullScreenMessageId);
            
            if (device != null && message != null)
            {
                device.FullScreenMessageId = fullScreenMessageId;
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
                device.FullScreenMessageId = null;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Cihaz bulunamadı");
            }
        }
    }
} 