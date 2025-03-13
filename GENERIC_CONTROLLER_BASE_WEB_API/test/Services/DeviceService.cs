using Microsoft.EntityFrameworkCore;
using test.Core;
using test.DTOs;
using test.Entities;

namespace test.Services
{
    public class DeviceService : BaseService<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto>
    {
        private readonly ApplicationDbContext _context;

        public DeviceService(IRepository<Device> repository, ApplicationDbContext context) 
            : base(repository)
        {
            _context = context;
        }

        protected override DeviceDto MapToDto(Device entity)
        {
            return new DeviceDto
            {
                Id = entity.Id,
                Name = entity.Name,
                SerialNumber = entity.SerialNumber,
                ApnName = _context.ApnNames.Find(entity.ApnNameId)?.Name,
                ApnPassword = _context.ApnPasswords.Find(entity.ApnPasswordId)?.Password,
                ApnAddress = _context.ApnAddresses.Find(entity.ApnAddressId)?.Address,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };
        }

        protected override Device MapToEntity(CreateDeviceDto createDto)
        {
            return new Device
            {
                Name = createDto.Name,
                SerialNumber = createDto.SerialNumber,
                ApnNameId = createDto.ApnNameId,
                ApnPasswordId = createDto.ApnPasswordId,
                ApnAddressId = createDto.ApnAddressId
            };
        }

        protected override Device MapToEntity(UpdateDeviceDto updateDto)
        {
            return new Device
            {
                Id = updateDto.Id,
                Name = updateDto.Name,
                SerialNumber = updateDto.SerialNumber,
                ApnNameId = updateDto.ApnNameId,
                ApnPasswordId = updateDto.ApnPasswordId,
                ApnAddressId = updateDto.ApnAddressId
            };
        }
    }
} 