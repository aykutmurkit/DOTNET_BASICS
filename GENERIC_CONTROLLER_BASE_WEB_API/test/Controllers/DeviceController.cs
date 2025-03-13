using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.DTOs;
using test.Entities;

namespace test.Controllers
{
    public class DeviceController : BaseController<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto>
    {
        public DeviceController(IService<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto> service)
            : base(service)
        {
        }
    }
} 