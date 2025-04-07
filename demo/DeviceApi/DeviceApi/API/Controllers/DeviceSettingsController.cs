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
    public class DeviceSettingsController : ControllerBase
    {
        private readonly IDeviceSettingsService _deviceSettingsService;
        private readonly ILogger<DeviceSettingsController> _logger;
        private readonly ILogService _logService;

        public DeviceSettingsController(
            IDeviceSettingsService deviceSettingsService,
            ILogger<DeviceSettingsController> logger,
            ILogService logService)
        {
            _deviceSettingsService = deviceSettingsService;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        /// Tüm cihaz ayarlarını getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<DeviceSettingsDto>>), 200)]
        public async Task<IActionResult> GetAllDeviceSettings()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetAllDeviceSettings çağrıldı", 
                "DeviceSettingsController.GetAllDeviceSettings", 
                new { UserId = userId, Role = userRole });
            
            var deviceSettings = await _deviceSettingsService.GetAllDeviceSettingsAsync();
            return Ok(ApiResponse<List<DeviceSettingsDto>>.Success(deviceSettings, "Cihaz ayarları başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre cihaz ayarlarını getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DeviceSettingsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetDeviceSettingsById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetDeviceSettingsById çağrıldı", 
                "DeviceSettingsController.GetDeviceSettingsById", 
                new { DeviceSettingsId = id, UserId = userId, Role = userRole });
            
            try
            {
                var deviceSettings = await _deviceSettingsService.GetDeviceSettingsByIdAsync(id);
                return Ok(ApiResponse<DeviceSettingsDto>.Success(deviceSettings, "Cihaz ayarları başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "DeviceSettingsId bulunamadı", 
                    "DeviceSettingsController.GetDeviceSettingsById", 
                    new { DeviceSettingsId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Cihaz ID'sine göre ayarları getirir
        /// </summary>
        [HttpGet("by-device/{deviceId}")]
        [ProducesResponseType(typeof(ApiResponse<DeviceSettingsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetDeviceSettingsByDeviceId(int deviceId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetDeviceSettingsByDeviceId çağrıldı", 
                "DeviceSettingsController.GetDeviceSettingsByDeviceId", 
                new { DeviceId = deviceId, UserId = userId, Role = userRole });
            
            try
            {
                var deviceSettings = await _deviceSettingsService.GetDeviceSettingsByDeviceIdAsync(deviceId);
                return Ok(ApiResponse<DeviceSettingsDto>.Success(deviceSettings, "Cihaz ayarları başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "DeviceId için ayarlar bulunamadı", 
                    "DeviceSettingsController.GetDeviceSettingsByDeviceId", 
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Cihaz için ayarlar oluşturur
        /// </summary>
        [HttpPost("{deviceId}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<DeviceSettingsDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreateDeviceSettings(int deviceId, [FromBody] CreateDeviceSettingsRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "CreateDeviceSettings çağrıldı", 
                "DeviceSettingsController.CreateDeviceSettings", 
                new { DeviceId = deviceId, UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "DeviceSettingsController.CreateDeviceSettings", 
                    new { DeviceId = deviceId, UserId = userId, Role = userRole, ModelErrors = ModelState.Values
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
                var createdDeviceSettings = await _deviceSettingsService.CreateDeviceSettingsAsync(deviceId, request);
                
                await _logService.LogInfoAsync(
                    "Cihaz ayarları oluşturuldu", 
                    "DeviceSettingsController.CreateDeviceSettings", 
                    new { DeviceSettingsId = createdDeviceSettings.Id, DeviceId = deviceId, UserId = userId, Role = userRole });
                    
                var response = ApiResponse<DeviceSettingsDto>.Created(createdDeviceSettings, "Cihaz ayarları başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetDeviceSettingsById), new { id = createdDeviceSettings.Id }, response);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Cihaz ayarları oluşturulurken hata", 
                    "DeviceSettingsController.CreateDeviceSettings", 
                    ex,
                    userId,
                    userRole);
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Cihaz ayarlarını günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<DeviceSettingsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdateDeviceSettings(int id, [FromBody] UpdateDeviceSettingsRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "UpdateDeviceSettings çağrıldı", 
                "DeviceSettingsController.UpdateDeviceSettings", 
                new { DeviceSettingsId = id, UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "DeviceSettingsController.UpdateDeviceSettings", 
                    new { DeviceSettingsId = id, UserId = userId, Role = userRole, ModelErrors = ModelState.Values
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
                var updatedDeviceSettings = await _deviceSettingsService.UpdateDeviceSettingsAsync(id, request);
                
                await _logService.LogInfoAsync(
                    "Cihaz ayarları güncellendi", 
                    "DeviceSettingsController.UpdateDeviceSettings", 
                    new { DeviceSettingsId = id, UserId = userId, Role = userRole });
                
                return Ok(ApiResponse<DeviceSettingsDto>.Success(updatedDeviceSettings, "Cihaz ayarları başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Cihaz ayarları güncellenirken hata", 
                    "DeviceSettingsController.UpdateDeviceSettings", 
                    ex,
                    userId,
                    userRole);
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Cihaz ayarlarını siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeleteDeviceSettings(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "DeleteDeviceSettings çağrıldı", 
                "DeviceSettingsController.DeleteDeviceSettings", 
                new { DeviceSettingsId = id, UserId = userId, Role = userRole });
            
            try
            {
                await _deviceSettingsService.DeleteDeviceSettingsAsync(id);
                
                await _logService.LogInfoAsync(
                    "Cihaz ayarları silindi", 
                    "DeviceSettingsController.DeleteDeviceSettings", 
                    new { DeviceSettingsId = id, UserId = userId, Role = userRole });
                
                return Ok(ApiResponse<object>.NoContent("Cihaz ayarları başarıyla silindi"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Cihaz ayarları silinirken hata", 
                    "DeviceSettingsController.DeleteDeviceSettings", 
                    ex,
                    userId,
                    userRole);
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
} 