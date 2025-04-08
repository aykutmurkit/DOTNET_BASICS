using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Cihaz durum repository implementasyonu
    /// </summary>
    public class DeviceStatusRepository : IDeviceStatusRepository
    {
        private readonly AppDbContext _context;

        public DeviceStatusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeviceStatus>> GetAllDeviceStatusesAsync()
        {
            return await _context.DeviceStatuses
                .Include(ds => ds.Device)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DeviceStatus> GetDeviceStatusByIdAsync(int id)
        {
            return await _context.DeviceStatuses
                .Include(ds => ds.Device)
                .FirstOrDefaultAsync(ds => ds.Id == id);
        }

        public async Task<DeviceStatus> GetDeviceStatusByDeviceIdAsync(int deviceId)
        {
            return await _context.DeviceStatuses
                .Include(ds => ds.Device)
                .FirstOrDefaultAsync(ds => ds.DeviceId == deviceId);
        }

        public async Task AddDeviceStatusAsync(DeviceStatus deviceStatus)
        {
            deviceStatus.CreatedAt = DateTime.Now;
            await _context.DeviceStatuses.AddAsync(deviceStatus);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDeviceStatusAsync(DeviceStatus deviceStatus)
        {
            deviceStatus.UpdatedAt = DateTime.Now;
            _context.DeviceStatuses.Update(deviceStatus);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDeviceStatusAsync(int id)
        {
            var deviceStatus = await _context.DeviceStatuses.FindAsync(id);
            if (deviceStatus != null)
            {
                _context.DeviceStatuses.Remove(deviceStatus);
                await _context.SaveChangesAsync();
            }
        }
    }
} 