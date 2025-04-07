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
    public class PeriodicMessagesController : ControllerBase
    {
        private readonly IPeriodicMessageService _periodicMessageService;
        private readonly ILogger<PeriodicMessagesController> _logger;
        private readonly ILogService _logService;

        public PeriodicMessagesController(
            IPeriodicMessageService periodicMessageService,
            ILogger<PeriodicMessagesController> logger,
            ILogService logService)
        {
            _periodicMessageService = periodicMessageService;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        /// Tüm periyodik mesajları getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<PeriodicMessageDto>>), 200)]
        public async Task<IActionResult> GetAllPeriodicMessages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "GetAllPeriodicMessages çağrıldı",
                "PeriodicMessagesController.GetAllPeriodicMessages",
                new { UserId = userId, Role = userRole });

            var messages = await _periodicMessageService.GetAllPeriodicMessagesAsync();
            return Ok(ApiResponse<List<PeriodicMessageDto>>.Success(messages, "Periyodik mesajlar başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre periyodik mesajı getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PeriodicMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetPeriodicMessageById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "GetPeriodicMessageById çağrıldı",
                "PeriodicMessagesController.GetPeriodicMessageById",
                new { MessageId = id, UserId = userId, Role = userRole });

            try
            {
                var message = await _periodicMessageService.GetPeriodicMessageByIdAsync(id);
                return Ok(ApiResponse<PeriodicMessageDto>.Success(message, "Periyodik mesaj başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Periyodik mesaj bulunamadı",
                    "PeriodicMessagesController.GetPeriodicMessageById",
                    new { MessageId = id, UserId = userId, Role = userRole });

                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Cihaz ID'sine göre periyodik mesajı getirir
        /// </summary>
        [HttpGet("by-device/{deviceId}")]
        [ProducesResponseType(typeof(ApiResponse<PeriodicMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetPeriodicMessageByDeviceId(int deviceId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "GetPeriodicMessageByDeviceId çağrıldı",
                "PeriodicMessagesController.GetPeriodicMessageByDeviceId",
                new { DeviceId = deviceId, UserId = userId, Role = userRole });

            try
            {
                var message = await _periodicMessageService.GetPeriodicMessageByDeviceIdAsync(deviceId);
                return Ok(ApiResponse<PeriodicMessageDto>.Success(message, $"Cihaza (ID: {deviceId}) ait periyodik mesaj başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Cihaza ait periyodik mesaj bulunamadı",
                    "PeriodicMessagesController.GetPeriodicMessageByDeviceId",
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });

                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Yeni periyodik mesaj oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<PeriodicMessageDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreatePeriodicMessage([FromBody] CreatePeriodicMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "CreatePeriodicMessage çağrıldı",
                "PeriodicMessagesController.CreatePeriodicMessage",
                new { UserId = userId, Role = userRole });

            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu",
                    "PeriodicMessagesController.CreatePeriodicMessage",
                    new { ModelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList());

                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var createdMessage = await _periodicMessageService.CreatePeriodicMessageAsync(request);

                await _logService.LogInfoAsync(
                    "Periyodik mesaj oluşturuldu",
                    "PeriodicMessagesController.CreatePeriodicMessage",
                    new { MessageId = createdMessage.Id, DeviceId = request.DeviceId, UserId = userId, Role = userRole });

                var response = ApiResponse<PeriodicMessageDto>.Created(createdMessage, "Periyodik mesaj başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetPeriodicMessageById), new { id = createdMessage.Id }, response);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Periyodik mesaj oluşturulurken hata",
                    "PeriodicMessagesController.CreatePeriodicMessage",
                    ex,
                    userId,
                    userRole);

                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Periyodik mesajı günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<PeriodicMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdatePeriodicMessage(int id, [FromBody] UpdatePeriodicMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "UpdatePeriodicMessage çağrıldı",
                "PeriodicMessagesController.UpdatePeriodicMessage",
                new { MessageId = id, UserId = userId, Role = userRole });

            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu",
                    "PeriodicMessagesController.UpdatePeriodicMessage",
                    new { ModelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList());

                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var updatedMessage = await _periodicMessageService.UpdatePeriodicMessageAsync(id, request);

                await _logService.LogInfoAsync(
                    "Periyodik mesaj güncellendi",
                    "PeriodicMessagesController.UpdatePeriodicMessage",
                    new { MessageId = id, UserId = userId, Role = userRole });

                return Ok(ApiResponse<PeriodicMessageDto>.Success(updatedMessage, "Periyodik mesaj başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Periyodik mesaj güncellenirken hata",
                    "PeriodicMessagesController.UpdatePeriodicMessage",
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
        /// Periyodik mesajı siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<object>), 204)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeletePeriodicMessage(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "DeletePeriodicMessage çağrıldı",
                "PeriodicMessagesController.DeletePeriodicMessage",
                new { MessageId = id, UserId = userId, Role = userRole });

            try
            {
                await _periodicMessageService.DeletePeriodicMessageAsync(id);

                await _logService.LogInfoAsync(
                    "Periyodik mesaj silindi",
                    "PeriodicMessagesController.DeletePeriodicMessage",
                    new { MessageId = id, UserId = userId, Role = userRole });

                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Periyodik mesaj silinirken hata",
                    "PeriodicMessagesController.DeletePeriodicMessage",
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