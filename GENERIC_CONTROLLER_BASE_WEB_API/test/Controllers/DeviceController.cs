using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.DTOs;
using test.Entities;
using test.Services;

namespace test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Device")]
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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public override async Task<ActionResult<IEnumerable<DeviceDto>>> GetAll()
        {
            return await base.GetAll();
        }

        /// <summary>
        /// Get device by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<ActionResult<DeviceDto>> GetById(int id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Get all devices with details
        /// </summary>
        [HttpGet("details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<ActionResult<IEnumerable<DeviceDetailDto>>> GetAllDetails()
        {
            var items = await _deviceService.GetAllDeviceDetailsAsync();
            return Ok(items);
        }

        /// <summary>
        /// Get device details by id
        /// </summary>
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<DeviceDetailDto>> GetDetailById(int id)
        {
            var item = await _deviceService.GetDeviceDetailByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        /// <summary>
        /// Create a new device
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<ActionResult<DeviceDto>> Create([FromBody] CreateDeviceDto createDto)
        {
            return await base.Create(createDto);
        }

        /// <summary>
        /// Update an existing device
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<ActionResult<DeviceDto>> Update(int id, [FromBody] UpdateDeviceDto updateDto)
        {
            return await base.Update(id, updateDto);
        }

        /// <summary>
        /// Delete a device
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<ActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
} 