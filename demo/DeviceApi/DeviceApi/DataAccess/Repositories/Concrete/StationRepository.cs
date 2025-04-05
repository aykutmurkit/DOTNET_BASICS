using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// İstasyon repository implementasyonu
    /// </summary>
    public class StationRepository : IStationRepository
    {
        private readonly AppDbContext _context;

        public StationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Station>> GetAllStationsWithRelationsAsync()
        {
            return await _context.Stations
                .Include(s => s.Platforms)
                    .ThenInclude(p => p.Devices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Station> GetStationByIdWithRelationsAsync(int id)
        {
            return await _context.Stations
                .Include(s => s.Platforms)
                    .ThenInclude(p => p.Devices)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Station> GetStationByIdAsync(int id)
        {
            return await _context.Stations.FindAsync(id);
        }

        public async Task AddStationAsync(Station station)
        {
            await _context.Stations.AddAsync(station);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStationAsync(Station station)
        {
            _context.Stations.Update(station);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStationAsync(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station != null)
            {
                _context.Stations.Remove(station);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> StationNameExistsAsync(string name)
        {
            return await _context.Stations.AnyAsync(s => s.Name == name);
        }
    }
} 