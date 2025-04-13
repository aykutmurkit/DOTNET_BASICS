using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Cihaz repository implementasyonu
    /// </summary>
    public class DeviceRepository : IDeviceRepository
    {
        private readonly AppDbContext _context;

        public DeviceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Device>> GetAllDevicesAsync()
        {
            return await _context.Devices
                .Include(d => d.Platform)
                    .ThenInclude(p => p.Station)
                .Include(d => d.Settings)
                .Include(d => d.Status)
                .Include(d => d.FullScreenMessage)
                .Include(d => d.ScrollingScreenMessage)
                .Include(d => d.BitmapScreenMessage)
                .Include(d => d.PeriodicMessage)
                .AsNoTracking()
                .ToListAsync();
        }
        
        public async Task<List<Device>> GetAllAsync()
        {
            // This is an alias for GetAllDevicesAsync to maintain compatibility
            return await GetAllDevicesAsync();
        }

        public async Task<Device> GetDeviceByIdAsync(int id)
        {
            return await _context.Devices
                .Include(d => d.Platform)
                    .ThenInclude(p => p.Station)
                .Include(d => d.Settings)
                .Include(d => d.Status)
                .Include(d => d.FullScreenMessage)
                .Include(d => d.ScrollingScreenMessage)
                .Include(d => d.BitmapScreenMessage)
                .Include(d => d.PeriodicMessage)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        
        public async Task<Device> GetByIdAsync(int id)
        {
            // This is an alias for GetDeviceByIdAsync to maintain compatibility
            return await GetDeviceByIdAsync(id);
        }
        
        public async Task<Device> GetByIdWithStatusAsync(int id)
        {
            return await _context.Devices
                .Include(d => d.Status)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        
        public async Task<List<Device>> GetDevicesByNameAsync(string name)
        {
            return await _context.Devices
                .Include(d => d.Platform)
                    .ThenInclude(p => p.Station)
                .Include(d => d.Settings)
                .Include(d => d.Status)
                .Include(d => d.FullScreenMessage)
                .Include(d => d.ScrollingScreenMessage)
                .Include(d => d.BitmapScreenMessage)
                .Include(d => d.PeriodicMessage)
                .Where(d => d.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<List<Device>> GetDevicesByPlatformIdAsync(int platformId)
        {
            return await _context.Devices
                .Include(d => d.Platform)
                    .ThenInclude(p => p.Station)
                .Include(d => d.Settings)
                .Include(d => d.Status)
                .Include(d => d.FullScreenMessage)
                .Include(d => d.ScrollingScreenMessage)
                .Include(d => d.BitmapScreenMessage)
                .Include(d => d.PeriodicMessage)
                .Where(d => d.PlatformId == platformId)
                .ToListAsync();
        }

        public async Task<List<Device>> GetDevicesByStationIdAsync(int stationId)
        {
            return await _context.Devices
                .Include(d => d.Platform)
                    .ThenInclude(p => p.Station)
                .Include(d => d.Settings)
                .Include(d => d.Status)
                .Include(d => d.FullScreenMessage)
                .Include(d => d.ScrollingScreenMessage)
                .Include(d => d.BitmapScreenMessage)
                .Include(d => d.PeriodicMessage)
                .Where(d => d.Platform.StationId == stationId)
                .ToListAsync();
        }

        public async Task AddDeviceAsync(Device device)
        {
            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDeviceAsync(Device device)
        {
            _context.Devices.Update(device);
            await _context.SaveChangesAsync();
        }
        
        public async Task<Device> UpdateAsync(Device device)
        {
            _context.Devices.Update(device);
            await _context.SaveChangesAsync();
            return device;
        }

        public async Task DeleteDeviceAsync(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<bool> IpPortCombinationExistsAsync(string ip, int port, int? excludeDeviceId = null)
        {
            var query = _context.Devices.Where(d => d.Ip == ip && d.Port == port);
            
            if (excludeDeviceId.HasValue)
            {
                query = query.Where(d => d.Id != excludeDeviceId.Value);
            }
            
            return await query.AnyAsync();
        }
        
        public async Task<bool> IpPortCombinationExistsForDifferentDeviceAsync(int deviceId, string ip, int port)
        {
            return await _context.Devices
                .Where(d => d.Id != deviceId && d.Ip == ip && d.Port == port)
                .AnyAsync();
        }
    }
} 