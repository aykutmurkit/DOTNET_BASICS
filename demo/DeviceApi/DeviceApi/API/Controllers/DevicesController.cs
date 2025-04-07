using Core.Utilities;
using DeviceApi.Business.Services.Interfaces;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LogLibrary.Core.Interfaces;

namespace DeviceApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DevicesController> _logger;
        private readonly ILogService _logService;

        public DevicesController(
            IDeviceService deviceService, 
            ILogger<DevicesController> logger,
            ILogService logService)
        {
            _deviceService = deviceService;
            _logger = logger;
            _logService = logService;
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
            
            // Use LogLibrary to log the request
            await _logService.LogInfoAsync(
                "GetAllDevices çağrıldı", 
                "DevicesController.GetAllDevices", 
                new { UserId = userId, Role = userRole });
            
            var devices = await _deviceService.GetAllDevicesAsync();
            return Ok(ApiResponse<List<DeviceDto>>.Success(devices, "Cihazlar başarıyla getirildi"));
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
            
            // Use LogLibrary to log the request
            await _logService.LogInfoAsync(
                "GetDeviceById çağrıldı", 
                "DevicesController.GetDeviceById", 
                new { DeviceId = id, UserId = userId, Role = userRole });
            
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                return Ok(ApiResponse<DeviceDto>.Success(device, "Cihaz başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                // Log warning with LogLibrary
                await _logService.LogWarningAsync(
                    "DeviceId bulunamadı", 
                    "DevicesController.GetDeviceById", 
                    new { DeviceId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
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
            
            try
            {
                var devices = await _deviceService.GetDevicesByPlatformIdAsync(platformId);
                return Ok(ApiResponse<List<DeviceDto>>.Success(devices, $"Platform (ID: {platformId}) cihazları başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Platform id ile cihazlar getirilirken hata", 
                    "DevicesController.GetDevicesByPlatformId", 
                    new { PlatformId = platformId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
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
            
            try
            {
                var devices = await _deviceService.GetDevicesByStationIdAsync(stationId);
                return Ok(ApiResponse<List<DeviceDto>>.Success(devices, $"İstasyon (ID: {stationId}) cihazları başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "İstasyon id ile cihazlar getirilirken hata", 
                    "DevicesController.GetDevicesByStationId", 
                    new { StationId = stationId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
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
            
            // Log the request
            await _logService.LogInfoAsync(
                "CreateDevice çağrıldı", 
                "DevicesController.CreateDevice", 
                new { UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                // Log warning about invalid model
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "DevicesController.CreateDevice", 
                    new { UserId = userId, Role = userRole, ModelErrors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList() });
                
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var createdDevice = await _deviceService.CreateDeviceAsync(request);
                
                // Log success
                await _logService.LogInfoAsync(
                    "Cihaz oluşturuldu", 
                    "DevicesController.CreateDevice", 
                    new { DeviceId = createdDevice.Id, UserId = userId, Role = userRole });
                    
                var response = ApiResponse<DeviceDto>.Created(createdDevice, "Cihaz başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetDeviceById), new { id = createdDevice.Id }, response);
            }
            catch (Exception ex)
            {
                // Log error
                await _logService.LogErrorAsync(
                    "Cihaz oluşturulurken hata", 
                    "DevicesController.CreateDevice", 
                    ex,
                    userId,
                    userRole);
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
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
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var updatedDevice = await _deviceService.UpdateDeviceAsync(id, request);
                _logger.LogInformation("Cihaz güncellendi: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}",
                    id, userId, userRole);
                    
                return Ok(ApiResponse<DeviceDto>.Success(updatedDevice, "Cihaz başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz güncellenirken hata: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}", 
                    id, userId, userRole);
                    
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
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
                    
                return Ok(ApiResponse<object>.NoContent("Cihaz başarıyla silindi"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz silinirken hata: DeviceId: {DeviceId}, UserId: {UserId}, Role: {Role}", 
                    id, userId, userRole);
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
} 