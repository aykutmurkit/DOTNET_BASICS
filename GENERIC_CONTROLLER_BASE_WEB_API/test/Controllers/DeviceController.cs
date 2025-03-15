using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.DTOs;
using test.Entities;
using test.Services;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Device Management")]
    public class DeviceController : BaseController<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto>
    {
        private readonly DeviceService _deviceService;

        public DeviceController(IService<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto> service)
            : base(service)
        {
            _deviceService = (DeviceService)service;
        }

        /// <summary>
        /// Get all devices
        /// </summary>
        [HttpGet("details")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        public override async Task<ActionResult> GetAll()
        {
            return await base.GetAll();
        }

        /// <summary>
        /// Get device by id
        /// </summary>
        [HttpGet("details/{id}")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public override async Task<ActionResult> GetById(int id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Get all devices with details
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        public virtual async Task<ActionResult> GetAllDetails()
        {
            var items = await _deviceService.GetAllDeviceDetailsAsync();
            return this.Ok(items, "Device details retrieved successfully");
        }

        /// <summary>
        /// Get device details by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult> GetDetailById(int id)
        {
            var item = await _deviceService.GetDeviceDetailByIdAsync(id);
            if (item == null)
                return this.NotFound($"Device details with ID {id} not found");

            return this.Ok(item, "Device details retrieved successfully");
        }

        /// <summary>
        /// Create a new device
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public override async Task<ActionResult> Create([FromBody] CreateDeviceDto createDto)
        {
            return await base.Create(createDto);
        }

        /// <summary>
        /// Update an existing device
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public override async Task<ActionResult> Update(int id, [FromBody] UpdateDeviceDto updateDto)
        {
            return await base.Update(id, updateDto);
        }

        /// <summary>
        /// Delete a device
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public override async Task<ActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
} 