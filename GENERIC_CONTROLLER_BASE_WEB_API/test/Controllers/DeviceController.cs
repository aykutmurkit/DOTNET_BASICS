using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.DTOs;
using test.Entities;
using test.Services;

namespace test.Controllers
{
    public class DeviceController : BaseController<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto>
    {
        private readonly DeviceService _deviceService;

        public DeviceController(IService<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto> service)
            : base(service)
        {
            _deviceService = (DeviceService)service;
        }

        [HttpGet("details")]
        public virtual async Task<ActionResult<IEnumerable<DeviceDetailDto>>> GetAllDetails()
        {
            var items = await _deviceService.GetAllDeviceDetailsAsync();
            return Ok(items);
        }

        [HttpGet("details/{id}")]
        public virtual async Task<ActionResult<DeviceDetailDto>> GetDetailById(int id)
        {
            var item = await _deviceService.GetDeviceDetailByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }
    }
} 