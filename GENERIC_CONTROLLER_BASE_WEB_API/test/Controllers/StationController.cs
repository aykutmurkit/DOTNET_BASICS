using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.DTOs;
using test.Entities;
using test.Services;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Station Management")]
    public class StationController : ControllerBase
    {
        private readonly StationService _stationService;

        public StationController(IServiceNonGeneric service)
        {
            _stationService = (StationService)service;
        }

        /// <summary>
        /// Get all stations
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<IEnumerable<StationDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll()
        {
            var items = await _stationService.GetAllAsync();
            return this.Ok(items, "Stations retrieved successfully");
        }

        /// <summary>
        /// Get station by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Result<StationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {
            var item = await _stationService.GetByIdAsync(id);
            if (item == null)
                return this.NotFound($"Station with ID {id} not found");

            return this.Ok(item, "Station retrieved successfully");
        }

        /// <summary>
        /// Create a new station
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<StationDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create([FromBody] CreateStationDto createDto)
        {
            if (!ModelState.IsValid)
                return this.BadRequest("Invalid data", ModelState.GetValidationErrors());
            
            var item = await _stationService.CreateStationAsync(createDto);
            return this.Created(item, "Station created successfully");
        }

        /// <summary>
        /// Update an existing station
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<StationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateStationDto updateDto)
        {
            if (id != updateDto.Id)
                return this.BadRequest("ID mismatch between URL and body");
            
            if (!ModelState.IsValid)
                return this.BadRequest("Invalid data", ModelState.GetValidationErrors());
            
            if (!await _stationService.ExistsAsync(id))
                return this.NotFound($"Station with ID {id} not found");
            
            var item = await _stationService.UpdateStationAsync(updateDto);
            return this.Ok(item, "Station updated successfully");
        }

        /// <summary>
        /// Delete a station
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            if (!await _stationService.ExistsAsync(id))
                return this.NotFound($"Station with ID {id} not found");

            await _stationService.DeleteAsync(id);
            return this.NoContent();
        }
    }
} 