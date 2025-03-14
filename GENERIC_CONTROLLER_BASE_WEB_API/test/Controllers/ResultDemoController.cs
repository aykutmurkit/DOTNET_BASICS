using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using test.Core;
using test.DTOs;
using test.Entities;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultDemoController : ControllerBase
    {
        private readonly IService<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto> _deviceService;

        public ResultDemoController(IService<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto> deviceService)
        {
            _deviceService = deviceService;
        }

        /// <summary>
        /// Get all devices with Result pattern
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<IEnumerable<DeviceDto>>), 200)]
        public async Task<ActionResult> GetAll()
        {
            var devices = await _deviceService.GetAllAsync();
            return this.Ok(devices, "Devices retrieved successfully");
        }

        /// <summary>
        /// Get device by id with Result pattern
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Result<DeviceDto>), 200)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<ActionResult> GetById(int id)
        {
            var device = await _deviceService.GetByIdAsync(id);
            if (device == null)
                return this.NotFound($"Device with ID {id} not found");

            return this.Ok(device, "Device retrieved successfully");
        }

        /// <summary>
        /// Create a new device with Result pattern
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<DeviceDto>), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<ActionResult> Create([FromBody] CreateDeviceDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest("Invalid device data", ModelState.GetValidationErrors());
            }

            var device = await _deviceService.CreateAsync(createDto);
            return this.Created(device, "Device created successfully");
        }

        /// <summary>
        /// Update an existing device with Result pattern
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<DeviceDto>), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateDeviceDto updateDto)
        {
            if (id != updateDto.Id)
                return this.BadRequest("ID mismatch between URL and body");

            if (!ModelState.IsValid)
            {
                return this.BadRequest("Invalid device data", ModelState.GetValidationErrors());
            }

            if (!await _deviceService.ExistsAsync(id))
                return this.NotFound($"Device with ID {id} not found");

            var device = await _deviceService.UpdateAsync(updateDto);
            return this.Ok(device, "Device updated successfully");
        }

        /// <summary>
        /// Delete a device with Result pattern
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result), 204)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<ActionResult> Delete(int id)
        {
            if (!await _deviceService.ExistsAsync(id))
                return this.NotFound($"Device with ID {id} not found");

            await _deviceService.DeleteAsync(id);
            return this.NoContent();
        }
    }
} 