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
    public class ScheduleRulesController : ControllerBase
    {
        private readonly IScheduleRuleService _scheduleRuleService;
        private readonly ILogger<ScheduleRulesController> _logger;
        private readonly ILogService _logService;

        public ScheduleRulesController(
            IScheduleRuleService scheduleRuleService,
            ILogger<ScheduleRulesController> logger,
            ILogService logService)
        {
            _scheduleRuleService = scheduleRuleService;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        /// Tüm zamanlanmış kuralları getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ScheduleRuleDto>>), 200)]
        public async Task<IActionResult> GetAllRules()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetAllRules çağrıldı", 
                "ScheduleRulesController.GetAllRules", 
                new { UserId = userId, Role = userRole });
            
            var rules = await _scheduleRuleService.GetAllRulesAsync();
            
            return Ok(ApiResponse<List<ScheduleRuleDto>>.Success(rules, "Zamanlanmış kurallar başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre zamanlanmış kuralı getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ScheduleRuleDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetRuleById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetRuleById çağrıldı", 
                "ScheduleRulesController.GetRuleById", 
                new { RuleId = id, UserId = userId, Role = userRole });
            
            try
            {
                var rule = await _scheduleRuleService.GetRuleByIdAsync(id);
                return Ok(ApiResponse<ScheduleRuleDto>.Success(rule, "Zamanlanmış kural başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Zamanlanmış kural bulunamadı", 
                    "ScheduleRulesController.GetRuleById", 
                    new { RuleId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Cihaz ID'sine göre zamanlanmış kuralları getirir
        /// </summary>
        [HttpGet("device/{deviceId}")]
        [ProducesResponseType(typeof(ApiResponse<List<ScheduleRuleDto>>), 200)]
        public async Task<IActionResult> GetRulesByDeviceId(int deviceId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetRulesByDeviceId çağrıldı", 
                "ScheduleRulesController.GetRulesByDeviceId", 
                new { DeviceId = deviceId, UserId = userId, Role = userRole });
            
            var rules = await _scheduleRuleService.GetRulesByDeviceIdAsync(deviceId);
            
            return Ok(ApiResponse<List<ScheduleRuleDto>>.Success(rules, $"Cihaz (ID: {deviceId}) için zamanlanmış kurallar başarıyla getirildi"));
        }

        /// <summary>
        /// Yeni zamanlanmış kural oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<ScheduleRuleDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreateRule([FromBody] CreateScheduleRuleDto createScheduleRuleDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "CreateRule çağrıldı", 
                "ScheduleRulesController.CreateRule", 
                new { DeviceId = createScheduleRuleDto.DeviceId, UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "ScheduleRulesController.CreateRule", 
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
                var createdRule = await _scheduleRuleService.CreateRuleAsync(createScheduleRuleDto);
                
                await _logService.LogInfoAsync(
                    "Zamanlanmış kural oluşturuldu", 
                    "ScheduleRulesController.CreateRule", 
                    new { RuleId = createdRule.Id, DeviceId = createdRule.DeviceId, UserId = userId, Role = userRole });
                
                var response = ApiResponse<ScheduleRuleDto>.Created(createdRule, "Zamanlanmış kural başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetRuleById), new { id = createdRule.Id }, response);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Zamanlanmış kural oluşturulurken hata", 
                    "ScheduleRulesController.CreateRule", 
                    ex,
                    userId,
                    userRole);
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Zamanlanmış kuralı günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<ScheduleRuleDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdateRule(int id, [FromBody] UpdateScheduleRuleDto updateScheduleRuleDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "UpdateRule çağrıldı", 
                "ScheduleRulesController.UpdateRule", 
                new { RuleId = id, DeviceId = updateScheduleRuleDto.DeviceId, UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "ScheduleRulesController.UpdateRule", 
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
                var updatedRule = await _scheduleRuleService.UpdateRuleAsync(id, updateScheduleRuleDto);
                
                await _logService.LogInfoAsync(
                    "Zamanlanmış kural güncellendi", 
                    "ScheduleRulesController.UpdateRule", 
                    new { RuleId = id, DeviceId = updatedRule.DeviceId, UserId = userId, Role = userRole });
                
                return Ok(ApiResponse<ScheduleRuleDto>.Success(updatedRule, "Zamanlanmış kural başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Zamanlanmış kural güncellenirken hata", 
                    "ScheduleRulesController.UpdateRule", 
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
        /// Zamanlanmış kuralı siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 204)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeleteRule(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "DeleteRule çağrıldı", 
                "ScheduleRulesController.DeleteRule", 
                new { RuleId = id, UserId = userId, Role = userRole });
            
            try
            {
                await _scheduleRuleService.DeleteRuleAsync(id);
                
                await _logService.LogInfoAsync(
                    "Zamanlanmış kural silindi", 
                    "ScheduleRulesController.DeleteRule", 
                    new { RuleId = id, UserId = userId, Role = userRole });
                
                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Zamanlanmış kural silinirken hata", 
                    "ScheduleRulesController.DeleteRule", 
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
        /// Tüm cihazlar için aktif kuralları uygular
        /// </summary>
        [HttpPost("apply-all")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        public async Task<IActionResult> ApplyAllRules()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "ApplyAllRules çağrıldı", 
                "ScheduleRulesController.ApplyAllRules", 
                new { UserId = userId, Role = userRole });
            
            try
            {
                await _scheduleRuleService.ApplyActiveRulesAsync();
                
                await _logService.LogInfoAsync(
                    "Tüm cihazlar için aktif kurallar uygulandı", 
                    "ScheduleRulesController.ApplyAllRules", 
                    new { UserId = userId, Role = userRole });
                
                return Ok(ApiResponse<object>.Success("Tüm cihazlar için aktif kurallar başarıyla uygulandı"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Aktif kurallar uygulanırken hata", 
                    "ScheduleRulesController.ApplyAllRules", 
                    ex,
                    userId,
                    userRole);
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Belirli bir cihaz için aktif kuralları uygular
        /// </summary>
        [HttpPost("apply-device/{deviceId}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> ApplyRulesForDevice(int deviceId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "ApplyRulesForDevice çağrıldı", 
                "ScheduleRulesController.ApplyRulesForDevice", 
                new { DeviceId = deviceId, UserId = userId, Role = userRole });
            
            try
            {
                await _scheduleRuleService.ApplyActiveRulesForDeviceAsync(deviceId);
                
                await _logService.LogInfoAsync(
                    "Cihaz için aktif kurallar uygulandı", 
                    "ScheduleRulesController.ApplyRulesForDevice", 
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });
                
                return Ok(ApiResponse<object>.Success($"Cihaz (ID: {deviceId}) için aktif kurallar başarıyla uygulandı"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Cihaz için aktif kurallar uygulanırken hata", 
                    "ScheduleRulesController.ApplyRulesForDevice", 
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