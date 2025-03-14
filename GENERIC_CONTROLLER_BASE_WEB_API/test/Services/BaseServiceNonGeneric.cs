using System.Linq.Expressions;
using test.Core;
using test.Entities;

namespace test.Services
{
    public abstract class BaseServiceNonGeneric : IServiceNonGeneric
    {
        protected readonly IRepositoryNonGeneric _repository;

        protected BaseServiceNonGeneric(IRepositoryNonGeneric repository)
        {
            _repository = repository;
        }

        public virtual async Task<IEnumerable<object>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public virtual async Task<object> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? MapToDto(entity) : null;
        }

        public virtual async Task<IEnumerable<object>> FindAsync(Expression<Func<BaseEntity, bool>> predicate)
        {
            var entities = await _repository.FindAsync(predicate);
            return entities.Select(MapToDto);
        }

        public virtual async Task<object> CreateAsync(object createDto)
        {
            var entity = MapToEntity(createDto, true);
            await _repository.AddAsync(entity);
            return MapToDto(entity);
        }

        public virtual async Task<object> UpdateAsync(object updateDto)
        {
            var entity = MapToEntity(updateDto, false);
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

        protected abstract object MapToDto(BaseEntity entity);
        protected abstract BaseEntity MapToEntity(object dto, bool isCreate);
    }
} 