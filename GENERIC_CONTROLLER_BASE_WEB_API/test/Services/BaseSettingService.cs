using test.Core;
using test.DTOs;
using test.Entities;

namespace test.Services
{
    public abstract class BaseSettingService<TEntity, TValue> : BaseService<TEntity, SettingDto<TValue>, CreateSettingDto<TValue>, UpdateSettingDto<TValue>>
        where TEntity : BaseSetting<TValue>
    {
        protected BaseSettingService(IRepository<TEntity> repository) : base(repository) { }

        protected override SettingDto<TValue> MapToDto(TEntity entity)
        {
            return new SettingDto<TValue>
            {
                Id = entity.Id,
                Value = entity.Value,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };
        }

        protected override TEntity MapToEntity(CreateSettingDto<TValue> createDto)
        {
            var entity = CreateNewEntity();
            entity.Value = createDto.Value;
            return entity;
        }

        protected override TEntity MapToEntity(UpdateSettingDto<TValue> updateDto)
        {
            var entity = CreateNewEntity();
            entity.Id = updateDto.Id;
            entity.Value = updateDto.Value;
            return entity;
        }

        protected abstract TEntity CreateNewEntity();
    }
} 