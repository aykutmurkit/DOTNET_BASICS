using test.Core;
using test.DTOs;
using test.Entities;

namespace test.Services
{
    public class StationService : BaseServiceNonGeneric
    {
        public StationService(IRepositoryNonGeneric repository) : base(repository) { }

        protected override object MapToDto(BaseEntity entity)
        {
            var station = (Station)entity;
            return new StationDto
            {
                Id = station.Id,
                Name = station.Name,
                Location = station.Location,
                Capacity = station.Capacity,
                IsActive = station.IsActive,
                CreatedDate = station.CreatedDate,
                UpdatedDate = station.UpdatedDate
            };
        }

        protected override BaseEntity MapToEntity(object dto, bool isCreate)
        {
            if (isCreate)
            {
                var createDto = (CreateStationDto)dto;
                return new Station
                {
                    Name = createDto.Name,
                    Location = createDto.Location,
                    Capacity = createDto.Capacity,
                    IsActive = createDto.IsActive
                };
            }
            else
            {
                var updateDto = (UpdateStationDto)dto;
                return new Station
                {
                    Id = updateDto.Id,
                    Name = updateDto.Name,
                    Location = updateDto.Location,
                    Capacity = updateDto.Capacity,
                    IsActive = updateDto.IsActive
                };
            }
        }
    }
} 