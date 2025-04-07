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
    public class ScrollingScreenMessagesController : ControllerBase
    {
        private readonly IScrollingScreenMessageService _scrollingScreenMessageService;
        private readonly ILogger<ScrollingScreenMessagesController> _logger;
        private readonly ILogService _logService;

        public ScrollingScreenMessagesController(
            IScrollingScreenMessageService scrollingScreenMessageService,
            ILogger<ScrollingScreenMessagesController> logger,
            ILogService logService)
        {
            _scrollingScreenMessageService = scrollingScreenMessageService;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        /// Tüm kayan ekran mesajlarını getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ScrollingScreenMessageDto>>), 200)]
        public async Task<IActionResult> GetAllScrollingScreenMessages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "GetAllScrollingScreenMessages çağrıldı",
                "ScrollingScreenMessagesController.GetAllScrollingScreenMessages",
                new { UserId = userId, Role = userRole });

            var messages = await _scrollingScreenMessageService.GetAllScrollingScreenMessagesAsync();
            return Ok(ApiResponse<List<ScrollingScreenMessageDto>>.Success(messages, "Kayan ekran mesajları başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre kayan ekran mesajı getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ScrollingScreenMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetScrollingScreenMessageById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "GetScrollingScreenMessageById çağrıldı",
                "ScrollingScreenMessagesController.GetScrollingScreenMessageById",
                new { MessageId = id, UserId = userId, Role = userRole });

            try
            {
                var message = await _scrollingScreenMessageService.GetScrollingScreenMessageByIdAsync(id);
                return Ok(ApiResponse<ScrollingScreenMessageDto>.Success(message, "Kayan ekran mesajı başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Kayan ekran mesajı bulunamadı",
                    "ScrollingScreenMessagesController.GetScrollingScreenMessageById",
                    new { MessageId = id, UserId = userId, Role = userRole });

                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Cihaz ID'sine göre kayan ekran mesajı getirir
        /// </summary>
        [HttpGet("by-device/{deviceId}")]
        [ProducesResponseType(typeof(ApiResponse<ScrollingScreenMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetScrollingScreenMessageByDeviceId(int deviceId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "GetScrollingScreenMessageByDeviceId çağrıldı",
                "ScrollingScreenMessagesController.GetScrollingScreenMessageByDeviceId",
                new { DeviceId = deviceId, UserId = userId, Role = userRole });

            try
            {
                var message = await _scrollingScreenMessageService.GetScrollingScreenMessageByDeviceIdAsync(deviceId);
                return Ok(ApiResponse<ScrollingScreenMessageDto>.Success(message, $"Cihaza (ID: {deviceId}) ait kayan ekran mesajı başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Cihaza ait kayan ekran mesajı bulunamadı",
                    "ScrollingScreenMessagesController.GetScrollingScreenMessageByDeviceId",
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });

                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Yeni kayan ekran mesajı oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<ScrollingScreenMessageDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreateScrollingScreenMessage([FromBody] CreateScrollingScreenMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "CreateScrollingScreenMessage çağrıldı",
                "ScrollingScreenMessagesController.CreateScrollingScreenMessage",
                new { UserId = userId, Role = userRole });

            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu",
                    "ScrollingScreenMessagesController.CreateScrollingScreenMessage",
                    new { ModelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList());

                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var createdMessage = await _scrollingScreenMessageService.CreateScrollingScreenMessageAsync(request);

                await _logService.LogInfoAsync(
                    "Kayan ekran mesajı oluşturuldu",
                    "ScrollingScreenMessagesController.CreateScrollingScreenMessage",
                    new { MessageId = createdMessage.Id, DeviceId = request.DeviceId, UserId = userId, Role = userRole });

                var response = ApiResponse<ScrollingScreenMessageDto>.Created(createdMessage, "Kayan ekran mesajı başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetScrollingScreenMessageById), new { id = createdMessage.Id }, response);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Kayan ekran mesajı oluşturulurken hata",
                    "ScrollingScreenMessagesController.CreateScrollingScreenMessage",
                    ex,
                    userId,
                    userRole);

                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Kayan ekran mesajını günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<ScrollingScreenMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdateScrollingScreenMessage(int id, [FromBody] UpdateScrollingScreenMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "UpdateScrollingScreenMessage çağrıldı",
                "ScrollingScreenMessagesController.UpdateScrollingScreenMessage",
                new { MessageId = id, UserId = userId, Role = userRole });

            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu",
                    "ScrollingScreenMessagesController.UpdateScrollingScreenMessage",
                    new { ModelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList());

                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var updatedMessage = await _scrollingScreenMessageService.UpdateScrollingScreenMessageAsync(id, request);

                await _logService.LogInfoAsync(
                    "Kayan ekran mesajı güncellendi",
                    "ScrollingScreenMessagesController.UpdateScrollingScreenMessage",
                    new { MessageId = id, UserId = userId, Role = userRole });

                return Ok(ApiResponse<ScrollingScreenMessageDto>.Success(updatedMessage, "Kayan ekran mesajı başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Kayan ekran mesajı güncellenirken hata",
                    "ScrollingScreenMessagesController.UpdateScrollingScreenMessage",
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
        /// Kayan ekran mesajını siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<object>), 204)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeleteScrollingScreenMessage(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "DeleteScrollingScreenMessage çağrıldı",
                "ScrollingScreenMessagesController.DeleteScrollingScreenMessage",
                new { MessageId = id, UserId = userId, Role = userRole });

            try
            {
                await _scrollingScreenMessageService.DeleteScrollingScreenMessageAsync(id);

                await _logService.LogInfoAsync(
                    "Kayan ekran mesajı silindi",
                    "ScrollingScreenMessagesController.DeleteScrollingScreenMessage",
                    new { MessageId = id, UserId = userId, Role = userRole });

                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Kayan ekran mesajı silinirken hata",
                    "ScrollingScreenMessagesController.DeleteScrollingScreenMessage",
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