using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Cihaz ayarları repository implementasyonu
    /// </summary>
    public class DeviceSettingsRepository : IDeviceSettingsRepository
    {
        private readonly AppDbContext _context;

        public DeviceSettingsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeviceSettings>> GetAllDeviceSettingsAsync()
        {
            return await _context.DeviceSettings
                .Include(ds => ds.Device)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DeviceSettings> GetDeviceSettingsByIdAsync(int id)
        {
            return await _context.DeviceSettings
                .Include(ds => ds.Device)
                .FirstOrDefaultAsync(ds => ds.Id == id);
        }

        public async Task<DeviceSettings> GetDeviceSettingsByDeviceIdAsync(int deviceId)
        {
            return await _context.DeviceSettings
                .Include(ds => ds.Device)
                .FirstOrDefaultAsync(ds => ds.DeviceId == deviceId);
        }

        public async Task AddDeviceSettingsAsync(DeviceSettings deviceSettings)
        {
            await _context.DeviceSettings.AddAsync(deviceSettings);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDeviceSettingsAsync(DeviceSettings deviceSettings)
        {
            _context.DeviceSettings.Update(deviceSettings);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDeviceSettingsAsync(int id)
        {
            var deviceSettings = await _context.DeviceSettings.FindAsync(id);
            if (deviceSettings != null)
            {
                _context.DeviceSettings.Remove(deviceSettings);
                await _context.SaveChangesAsync();
            }
        }
    }
} 