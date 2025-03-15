using test.Core;
using test.DTOs;
using test.Entities;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

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
                CreateStationDto createDto;
                
                if (dto is JsonElement jsonElement)
                {
                    createDto = JsonSerializer.Deserialize<CreateStationDto>(
                        jsonElement.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                else
                {
                    createDto = (CreateStationDto)dto;
                }
                
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
                UpdateStationDto updateDto;
                
                if (dto is JsonElement jsonElement)
                {
                    updateDto = JsonSerializer.Deserialize<UpdateStationDto>(
                        jsonElement.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                else
                {
                    updateDto = (UpdateStationDto)dto;
                }
                
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

        // Direct methods for the controller
        public async Task<IEnumerable<StationDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => (StationDto)MapToDto(e));
        }

        public async Task<StationDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? (StationDto)MapToDto(entity) : null;
        }

        public async Task<StationDto> CreateStationAsync(CreateStationDto createDto)
        {
            var entity = new Station
            {
                Name = createDto.Name,
                Location = createDto.Location,
                Capacity = createDto.Capacity,
                IsActive = createDto.IsActive
            };
            
            await _repository.AddAsync(entity);
            return (StationDto)MapToDto(entity);
        }

        public async Task<StationDto> UpdateStationAsync(UpdateStationDto updateDto)
        {
            var entity = new Station
            {
                Id = updateDto.Id,
                Name = updateDto.Name,
                Location = updateDto.Location,
                Capacity = updateDto.Capacity,
                IsActive = updateDto.IsActive
            };
            
            await _repository.UpdateAsync(entity);
            return (StationDto)MapToDto(entity);
        }
    }
} 