using System.Linq.Expressions;
using test.Entities;

namespace test.Core
{
    public interface IServiceNonGeneric
    {
        Task<IEnumerable<object>> GetAllAsync();
        Task<object> GetByIdAsync(int id);
        Task<IEnumerable<object>> FindAsync(Expression<Func<BaseEntity, bool>> predicate);
        Task<object> CreateAsync(object createDto);
        Task<object> UpdateAsync(object updateDto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
} 