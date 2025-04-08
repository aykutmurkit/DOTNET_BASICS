using Core.Utilities;
using DeviceApi.Business.Services.Interfaces;
using Entities.Dtos;
using LogLibrary.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeviceApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeviceStatusesController : ControllerBase
    {
        private readonly IDeviceStatusService _deviceStatusService;
        private readonly ILogger<DeviceStatusesController> _logger;
        private readonly ILogService _logService;

        public DeviceStatusesController(
            IDeviceStatusService deviceStatusService,
            ILogger<DeviceStatusesController> logger,
            ILogService logService)
        {
            _deviceStatusService = deviceStatusService;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        /// Tüm cihaz durumlarını getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<DeviceStatusDto>>), 200)]
        public async Task<IActionResult> GetAllDeviceStatuses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetAllDeviceStatuses çağrıldı", 
                "DeviceStatusesController.GetAllDeviceStatuses", 
                new { UserId = userId, Role = userRole });
            
            var deviceStatuses = await _deviceStatusService.GetAllDeviceStatusesAsync();
            
            return Ok(ApiResponse<List<DeviceStatusDto>>.Success(deviceStatuses, "Cihaz durumları başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre cihaz durumu getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DeviceStatusDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetDeviceStatusById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetDeviceStatusById çağrıldı", 
                "DeviceStatusesController.GetDeviceStatusById", 
                new { DeviceStatusId = id, UserId = userId, Role = userRole });
            
            try 
            {
                var deviceStatus = await _deviceStatusService.GetDeviceStatusByIdAsync(id);
                return Ok(ApiResponse<DeviceStatusDto>.Success(deviceStatus, "Cihaz durumu başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Cihaz durumu bulunamadı", 
                    "DeviceStatusesController.GetDeviceStatusById", 
                    new { DeviceStatusId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Cihaz durumu bulunamadı"));
            }
        }

        /// <summary>
        /// Cihaz ID'sine göre cihaz durumu getirir
        /// </summary>
        [HttpGet("device/{deviceId}")]
        [ProducesResponseType(typeof(ApiResponse<DeviceStatusDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetDeviceStatusByDeviceId(int deviceId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetDeviceStatusByDeviceId çağrıldı", 
                "DeviceStatusesController.GetDeviceStatusByDeviceId", 
                new { DeviceId = deviceId, UserId = userId, Role = userRole });
            
            try 
            {
                var deviceStatus = await _deviceStatusService.GetDeviceStatusByDeviceIdAsync(deviceId);
                return Ok(ApiResponse<DeviceStatusDto>.Success(deviceStatus, "Cihaz durumu başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Cihaz durumu bulunamadı", 
                    "DeviceStatusesController.GetDeviceStatusByDeviceId", 
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Cihaz durumu bulunamadı"));
            }
        }

        /// <summary>
        /// Yeni cihaz durumu ekler
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<DeviceStatusDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> AddDeviceStatus(CreateDeviceStatusDto createDeviceStatusDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "AddDeviceStatus çağrıldı", 
                "DeviceStatusesController.AddDeviceStatus", 
                new { DeviceId = createDeviceStatusDto.DeviceId, UserId = userId, Role = userRole });
            
            try 
            {
                var deviceStatus = await _deviceStatusService.AddDeviceStatusAsync(createDeviceStatusDto);
                
                await _logService.LogInfoAsync(
                    "Cihaz durumu eklendi", 
                    "DeviceStatusesController.AddDeviceStatus", 
                    new { DeviceStatusId = deviceStatus.Id, DeviceId = deviceStatus.DeviceId, UserId = userId, Role = userRole });
                
                var response = ApiResponse<DeviceStatusDto>.Created(deviceStatus, "Cihaz durumu başarıyla eklendi");
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Cihaz durumu eklenirken hata oluştu", 
                    "DeviceStatusesController.AddDeviceStatus", 
                    ex,
                    userId,
                    userRole);
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Cihaz durumunu günceller
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DeviceStatusDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdateDeviceStatus(int id, UpdateDeviceStatusDto updateDeviceStatusDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "UpdateDeviceStatus çağrıldı", 
                "DeviceStatusesController.UpdateDeviceStatus", 
                new { DeviceStatusId = id, DeviceId = updateDeviceStatusDto.DeviceId, UserId = userId, Role = userRole });
            
            try 
            {
                var deviceStatus = await _deviceStatusService.UpdateDeviceStatusAsync(id, updateDeviceStatusDto);
                
                await _logService.LogInfoAsync(
                    "Cihaz durumu güncellendi", 
                    "DeviceStatusesController.UpdateDeviceStatus", 
                    new { DeviceStatusId = id, DeviceId = deviceStatus.DeviceId, UserId = userId, Role = userRole });
                
                return Ok(ApiResponse<DeviceStatusDto>.Success(deviceStatus, "Cihaz durumu başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Cihaz durumu güncellenirken hata oluştu", 
                    "DeviceStatusesController.UpdateDeviceStatus", 
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
        /// Cihaz durumunu siler
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<>), 204)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeleteDeviceStatus(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "DeleteDeviceStatus çağrıldı", 
                "DeviceStatusesController.DeleteDeviceStatus", 
                new { DeviceStatusId = id, UserId = userId, Role = userRole });
            
            try 
            {
                await _deviceStatusService.DeleteDeviceStatusAsync(id);
                
                await _logService.LogInfoAsync(
                    "Cihaz durumu silindi", 
                    "DeviceStatusesController.DeleteDeviceStatus", 
                    new { DeviceStatusId = id, UserId = userId, Role = userRole });
                
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Cihaz durumu silinirken hata oluştu", 
                    "DeviceStatusesController.DeleteDeviceStatus", 
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