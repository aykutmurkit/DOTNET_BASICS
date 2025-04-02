using Core.Utilities;
using DeviceApi.Business.Services.Interfaces;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeviceApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(IDeviceService deviceService, ILogger<DevicesController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        /// <summary>
        /// Tüm cihazları getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<DeviceDto>>), 200)]
        public async Task<IActionResult> GetAllDevices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("GetAllDevices çağrıldı: UserId: {UserId}, Role: {Role}", userId, userRole);
            
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("GetDeviceById çağrıldı: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}", id, userId, userRole);
            
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                _logger.LogWarning("DeviceId {DeviceId} bulunamadı: UserId: {UserId}, Role: {Role}", id, userId, userRole);
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("GetDevicesByPlatformId çağrıldı: PlatformId: {PlatformId}, UserId: {UserId}, Role: {Role}", 
                platformId, userId, userRole);
            
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("GetDevicesByStationId çağrıldı: StationId: {StationId}, UserId: {UserId}, Role: {Role}", 
                stationId, userId, userRole);
            
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("CreateDevice çağrıldı: UserId: {UserId}, Role: {Role}", userId, userRole);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Geçersiz model durumu: UserId: {UserId}, Role: {Role}", userId, userRole);
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Geçersiz veri"
                });
            }

            try
            {
                var createdDevice = await _deviceService.CreateDeviceAsync(request);
                _logger.LogInformation("Cihaz oluşturuldu: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}",
                    createdDevice.Id, userId, userRole);
                    
                return CreatedAtAction(nameof(GetDeviceById), new { id = createdDevice.Id }, new ApiResponse<DeviceDto>
                {
                    Data = createdDevice,
                    Message = "Cihaz başarıyla oluşturuldu"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz oluşturulurken hata: UserId: {UserId}, Role: {Role}", userId, userRole);
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("UpdateDevice çağrıldı: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}", 
                id, userId, userRole);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Geçersiz model durumu: UserId: {UserId}, Role: {Role}", userId, userRole);
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Geçersiz veri"
                });
            }

            try
            {
                var updatedDevice = await _deviceService.UpdateDeviceAsync(id, request);
                _logger.LogInformation("Cihaz güncellendi: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}",
                    id, userId, userRole);
                    
                return Ok(new ApiResponse<DeviceDto>
                {
                    Data = updatedDevice,
                    Message = "Cihaz başarıyla güncellendi"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz güncellenirken hata: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}", 
                    id, userId, userRole);
                    
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("DeleteDevice çağrıldı: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}", 
                id, userId, userRole);
            
            try
            {
                await _deviceService.DeleteDeviceAsync(id);
                _logger.LogInformation("Cihaz silindi: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}",
                    id, userId, userRole);
                    
                return Ok(new ApiResponse<object>
                {
                    Message = "Cihaz başarıyla silindi"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz silinirken hata: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}", 
                    id, userId, userRole);
                    
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