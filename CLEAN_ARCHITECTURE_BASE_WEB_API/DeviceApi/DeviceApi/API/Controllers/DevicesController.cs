using Core.Utilities;
using DeviceApi.Business.Services.Interfaces;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        /// <summary>
        /// Tüm cihazları getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<DeviceDto>>), 200)]
        public async Task<IActionResult> GetAllDevices()
        {
            var devices = await _deviceService.GetAllDevicesAsync();
            return Ok(new ApiResponse<List<DeviceDto>>
            {
                Data = devices,
                Message = "Cihazlar başarıyla getirildi"
            });
        }

        /// <summary>
        /// ID'ye göre cihaz getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DeviceDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetDeviceById(int id)
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Cihaz bulunamadı"
                });
            }
            
            return Ok(new ApiResponse<DeviceDto>
            {
                Data = device,
                Message = "Cihaz başarıyla getirildi"
            });
        }

        /// <summary>
        /// Platform ID'sine göre cihazları getirir
        /// </summary>
        [HttpGet("by-platform/{platformId}")]
        [ProducesResponseType(typeof(ApiResponse<List<DeviceDto>>), 200)]
        public async Task<IActionResult> GetDevicesByPlatformId(int platformId)
        {
            var devices = await _deviceService.GetDevicesByPlatformIdAsync(platformId);
            return Ok(new ApiResponse<List<DeviceDto>>
            {
                Data = devices,
                Message = $"Platform (ID: {platformId}) cihazları başarıyla getirildi"
            });
        }

        /// <summary>
        /// İstasyon ID'sine göre cihazları getirir
        /// </summary>
        [HttpGet("by-station/{stationId}")]
        [ProducesResponseType(typeof(ApiResponse<List<DeviceDto>>), 200)]
        public async Task<IActionResult> GetDevicesByStationId(int stationId)
        {
            var devices = await _deviceService.GetDevicesByStationIdAsync(stationId);
            return Ok(new ApiResponse<List<DeviceDto>>
            {
                Data = devices,
                Message = $"İstasyon (ID: {stationId}) cihazları başarıyla getirildi"
            });
        }

        /// <summary>
        /// Yeni cihaz oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<DeviceDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreateDevice([FromBody] CreateDeviceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Geçersiz veri"
                });
            }

            try
            {
                var createdDevice = await _deviceService.CreateDeviceAsync(request);
                return CreatedAtAction(nameof(GetDeviceById), new { id = createdDevice.Id }, new ApiResponse<DeviceDto>
                {
                    Data = createdDevice,
                    Message = "Cihaz başarıyla oluşturuldu"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Cihaz günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<DeviceDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdateDevice(int id, [FromBody] UpdateDeviceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Geçersiz veri"
                });
            }

            try
            {
                var updatedDevice = await _deviceService.UpdateDeviceAsync(id, request);
                return Ok(new ApiResponse<DeviceDto>
                {
                    Data = updatedDevice,
                    Message = "Cihaz başarıyla güncellendi"
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    });
                }
                
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Cihaz siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            try
            {
                await _deviceService.DeleteDeviceAsync(id);
                return Ok(new ApiResponse<object>
                {
                    Message = "Cihaz başarıyla silindi"
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    });
                }
                
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
    }
} 