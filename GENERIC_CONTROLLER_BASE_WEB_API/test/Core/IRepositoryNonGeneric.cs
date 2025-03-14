using System.Linq.Expressions;
using test.Entities;

namespace test.Core
{
    public interface IRepositoryNonGeneric
    {
        Task<IEnumerable<BaseEntity>> GetAllAsync();
        Task<BaseEntity> GetByIdAsync(int id);
        Task<IEnumerable<BaseEntity>> FindAsync(Expression<Func<BaseEntity, bool>> predicate);
        Task<BaseEntity> AddAsync(BaseEntity entity);
        Task<BaseEntity> UpdateAsync(BaseEntity entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
} 