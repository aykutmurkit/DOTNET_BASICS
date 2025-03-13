using System.Linq.Expressions;
using test.Core;
using test.Entities;

namespace test.Services
{
    public abstract class BaseService<T, TDto, TCreateDto, TUpdateDto> : IService<T, TDto, TCreateDto, TUpdateDto>
        where T : BaseEntity
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly IRepository<T> _repository;

        protected BaseService(IRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public virtual async Task<TDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? MapToDto(entity) : null;
        }

        public virtual async Task<IEnumerable<TDto>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var entities = await _repository.FindAsync(predicate);
            return entities.Select(MapToDto);
        }

        public virtual async Task<TDto> CreateAsync(TCreateDto createDto)
        {
            var entity = MapToEntity(createDto);
            await _repository.AddAsync(entity);
            return MapToDto(entity);
        }

        public virtual async Task<TDto> UpdateAsync(TUpdateDto updateDto)
        {
            var entity = MapToEntity(updateDto);
            await _repository.UpdateAsync(entity);
            return MapToDto(entity);
        }

        public virtual async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        protected abstract TDto MapToDto(T entity);
        protected abstract T MapToEntity(TCreateDto createDto);
        protected abstract T MapToEntity(TUpdateDto updateDto);
    }
} 