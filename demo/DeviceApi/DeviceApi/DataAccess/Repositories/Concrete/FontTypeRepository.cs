using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Font türü repository implementasyonu
    /// </summary>
    public class FontTypeRepository : IFontTypeRepository
    {
        private readonly AppDbContext _context;

        public FontTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FontType>> GetAllFontTypesAsync()
        {
            return await _context.FontTypes
                .Include(f => f.FullScreenMessages)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FontType> GetFontTypeByIdAsync(int id)
        {
            return await _context.FontTypes
                .Include(f => f.FullScreenMessages)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FontType> GetFontTypeByKeyAsync(int key)
        {
            return await _context.FontTypes
                .Include(f => f.FullScreenMessages)
                .FirstOrDefaultAsync(f => f.Key == key);
        }

        public async Task AddFontTypeAsync(FontType fontType)
        {
            await _context.FontTypes.AddAsync(fontType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFontTypeAsync(FontType fontType)
        {
            _context.FontTypes.Update(fontType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFontTypeAsync(int id)
        {
            var fontType = await _context.FontTypes.FindAsync(id);
            if (fontType != null)
            {
                _context.FontTypes.Remove(fontType);
                await _context.SaveChangesAsync();
            }
        }
    }
} 