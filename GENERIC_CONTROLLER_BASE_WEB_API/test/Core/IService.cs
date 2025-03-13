using System.Linq.Expressions;
using test.Entities;

namespace test.Core
{
    public interface IService<T, TDto, TCreateDto, TUpdateDto>
        where T : BaseEntity
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(int id);
        Task<IEnumerable<TDto>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<TDto> CreateAsync(TCreateDto createDto);
        Task<TDto> UpdateAsync(TUpdateDto updateDto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
} 