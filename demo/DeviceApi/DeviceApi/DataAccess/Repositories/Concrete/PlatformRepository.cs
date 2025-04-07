using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Platform repository implementasyonu
    /// </summary>
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _context;

        public PlatformRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Platform>> GetAllPlatformsWithDevicesAsync()
        {
            return await _context.Platforms
                .Include(p => p.Station)
                .Include(p => p.Devices)
                .Include(p => p.Prediction)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Platform> GetPlatformByIdWithDevicesAsync(int id)
        {
            return await _context.Platforms
                .Include(p => p.Station)
                .Include(p => p.Devices)
                .Include(p => p.Prediction)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Platform> GetPlatformByIdAsync(int id)
        {
            return await _context.Platforms
                .Include(p => p.Prediction)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Platform>> GetPlatformsByStationIdAsync(int stationId)
        {
            return await _context.Platforms
                .Include(p => p.Devices)
                .Include(p => p.Prediction)
                .Where(p => p.StationId == stationId)
                .ToListAsync();
        }

        public async Task AddPlatformAsync(Platform platform)
        {
            await _context.Platforms.AddAsync(platform);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePlatformAsync(Platform platform)
        {
            _context.Platforms.Update(platform);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePlatformAsync(int id)
        {
            var platform = await _context.Platforms.FindAsync(id);
            if (platform != null)
            {
                _context.Platforms.Remove(platform);
                await _context.SaveChangesAsync();
            }
        }
    }
} 