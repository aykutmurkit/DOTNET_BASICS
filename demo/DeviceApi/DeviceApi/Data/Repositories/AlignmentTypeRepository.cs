using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <summary>
    /// Hizalama türleri için repository implementasyonu
    /// </summary>
    public class AlignmentTypeRepository : IAlignmentTypeRepository
    {
        private readonly AppDbContext _context;

        public AlignmentTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tüm hizalama türlerini getirir
        /// </summary>
        public async Task<List<AlignmentType>> GetAllAlignmentTypesAsync()
        {
            return await _context.AlignmentTypes.ToListAsync();
        }

        /// <summary>
        /// ID'ye göre hizalama türü getirir
        /// </summary>
        public async Task<AlignmentType> GetAlignmentTypeByIdAsync(int id)
        {
            return await _context.AlignmentTypes.FindAsync(id);
        }

        /// <summary>
        /// Key değerine göre hizalama türü getirir
        /// </summary>
        public async Task<AlignmentType> GetAlignmentTypeByKeyAsync(int key)
        {
            return await _context.AlignmentTypes.FirstOrDefaultAsync(a => a.Key == key);
        }

        /// <summary>
        /// Hizalama türü ekler
        /// </summary>
        public async Task AddAlignmentTypeAsync(AlignmentType alignmentType)
        {
            await _context.AlignmentTypes.AddAsync(alignmentType);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Hizalama türü günceller
        /// </summary>
        public async Task UpdateAlignmentTypeAsync(AlignmentType alignmentType)
        {
            _context.AlignmentTypes.Update(alignmentType);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Hizalama türü siler
        /// </summary>
        public async Task DeleteAlignmentTypeAsync(int id)
        {
            var alignmentType = await _context.AlignmentTypes.FindAsync(id);
            if (alignmentType != null)
            {
                _context.AlignmentTypes.Remove(alignmentType);
                await _context.SaveChangesAsync();
            }
        }
    }
} 