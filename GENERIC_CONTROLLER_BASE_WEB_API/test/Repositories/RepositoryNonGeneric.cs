using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using test.Core;
using test.Entities;

namespace test.Repositories
{
    public class RepositoryNonGeneric : IRepositoryNonGeneric
    {
        protected readonly ApplicationDbContext _context;

        public RepositoryNonGeneric(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<IEnumerable<BaseEntity>> GetAllAsync()
        {
            var stations = await _context.Stations.Where(x => !x.IsDeleted).ToListAsync();
            return stations;
        }

        public virtual async Task<BaseEntity> GetByIdAsync(int id)
        {
            return await _context.Stations.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public virtual async Task<IEnumerable<BaseEntity>> FindAsync(Expression<Func<BaseEntity, bool>> predicate)
        {
            return await _context.Stations.Where(x => !x.IsDeleted).Where(predicate).ToListAsync();
        }

        public virtual async Task<BaseEntity> AddAsync(BaseEntity entity)
        {
            await _context.Stations.AddAsync((Station)entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<BaseEntity> UpdateAsync(BaseEntity entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _context.Stations.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }
    }
} 