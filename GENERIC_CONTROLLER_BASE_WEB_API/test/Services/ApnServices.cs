using test.Core;
using test.DTOs;
using test.Entities;

namespace test.Services
{
    public class ApnNameService : BaseService<ApnName, ApnNameDto, CreateApnNameDto, UpdateApnNameDto>
    {
        public ApnNameService(IRepository<ApnName> repository) : base(repository) { }

        protected override ApnNameDto MapToDto(ApnName entity)
        {
            return new ApnNameDto
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };
        }

        protected override ApnName MapToEntity(CreateApnNameDto createDto)
        {
            return new ApnName { Name = createDto.Name };
        }

        protected override ApnName MapToEntity(UpdateApnNameDto updateDto)
        {
            return new ApnName { Id = updateDto.Id, Name = updateDto.Name };
        }
    }

    public class ApnPasswordService : BaseService<ApnPassword, ApnPasswordDto, CreateApnPasswordDto, UpdateApnPasswordDto>
    {
        public ApnPasswordService(IRepository<ApnPassword> repository) : base(repository) { }

        protected override ApnPasswordDto MapToDto(ApnPassword entity)
        {
            return new ApnPasswordDto
            {
                Id = entity.Id,
                Password = entity.Password,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };
        }

        protected override ApnPassword MapToEntity(CreateApnPasswordDto createDto)
        {
            return new ApnPassword { Password = createDto.Password };
        }

        protected override ApnPassword MapToEntity(UpdateApnPasswordDto updateDto)
        {
            return new ApnPassword { Id = updateDto.Id, Password = updateDto.Password };
        }
    }

    public class ApnAddressService : BaseService<ApnAddress, ApnAddressDto, CreateApnAddressDto, UpdateApnAddressDto>
    {
        public ApnAddressService(IRepository<ApnAddress> repository) : base(repository) { }

        protected override ApnAddressDto MapToDto(ApnAddress entity)
        {
            return new ApnAddressDto
            {
                Id = entity.Id,
                Address = entity.Address,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };
        }

        protected override ApnAddress MapToEntity(CreateApnAddressDto createDto)
        {
            return new ApnAddress { Address = createDto.Address };
        }

        protected override ApnAddress MapToEntity(UpdateApnAddressDto updateDto)
        {
            return new ApnAddress { Id = updateDto.Id, Address = updateDto.Address };
        }
    }
} 