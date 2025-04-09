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
    public class BitmapScreenMessagesController : ControllerBase
    {
        private readonly IBitmapScreenMessageService _bitmapScreenMessageService;
        private readonly ILogger<BitmapScreenMessagesController> _logger;
        private readonly ILogService _logService;

        public BitmapScreenMessagesController(
            IBitmapScreenMessageService bitmapScreenMessageService,
            ILogger<BitmapScreenMessagesController> logger,
            ILogService logService)
        {
            _bitmapScreenMessageService = bitmapScreenMessageService;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        /// Tüm bitmap ekran mesajlarını getirir
        /// </summary>
        /// <returns>Başarılı durumda 200 OK ve bitmap ekran mesajları listesi</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<BitmapScreenMessageDto>>), 200)]
        public async Task<IActionResult> GetAllBitmapScreenMessages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "GetAllBitmapScreenMessages çağrıldı",
                "BitmapScreenMessagesController.GetAllBitmapScreenMessages",
                new { UserId = userId, Role = userRole });

            var messages = await _bitmapScreenMessageService.GetAllBitmapScreenMessagesAsync();
            return Ok(ApiResponse<List<BitmapScreenMessageDto>>.Success(messages, "Bitmap ekran mesajları başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre bitmap ekran mesajı getirir
        /// </summary>
        /// <param name="id">Bitmap ekran mesaj ID'si</param>
        /// <returns>Başarılı durumda 200 OK ve bitmap ekran mesajı, bulunamazsa 404 Not Found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BitmapScreenMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetBitmapScreenMessageById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "GetBitmapScreenMessageById çağrıldı",
                "BitmapScreenMessagesController.GetBitmapScreenMessageById",
                new { MessageId = id, UserId = userId, Role = userRole });

            try
            {
                var message = await _bitmapScreenMessageService.GetBitmapScreenMessageByIdAsync(id);
                return Ok(ApiResponse<BitmapScreenMessageDto>.Success(message, "Bitmap ekran mesajı başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Bitmap ekran mesajı bulunamadı",
                    "BitmapScreenMessagesController.GetBitmapScreenMessageById",
                    new { MessageId = id, UserId = userId, Role = userRole });

                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Cihaz ID'sine göre bitmap ekran mesajı getirir
        /// </summary>
        /// <param name="deviceId">Cihaz ID'si</param>
        /// <returns>Başarılı durumda 200 OK ve bitmap ekran mesajı, bulunamazsa 404 Not Found</returns>
        [HttpGet("by-device/{deviceId}")]
        [ProducesResponseType(typeof(ApiResponse<BitmapScreenMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetBitmapScreenMessageByDeviceId(int deviceId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "GetBitmapScreenMessageByDeviceId çağrıldı",
                "BitmapScreenMessagesController.GetBitmapScreenMessageByDeviceId",
                new { DeviceId = deviceId, UserId = userId, Role = userRole });

            try
            {
                var message = await _bitmapScreenMessageService.GetBitmapScreenMessageByDeviceIdAsync(deviceId);
                return Ok(ApiResponse<BitmapScreenMessageDto>.Success(message, $"Cihaza (ID: {deviceId}) ait bitmap ekran mesajı başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Cihaza ait bitmap ekran mesajı bulunamadı",
                    "BitmapScreenMessagesController.GetBitmapScreenMessageByDeviceId",
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });

                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Yeni bitmap ekran mesajı oluşturur
        /// </summary>
        /// <param name="request">Bitmap ekran mesaj oluşturma isteği</param>
        /// <returns>Başarılı durumda 201 Created ve oluşturulan bitmap ekran mesajı, hata durumunda 400 Bad Request</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<BitmapScreenMessageDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreateBitmapScreenMessage([FromBody] CreateBitmapScreenMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "CreateBitmapScreenMessage çağrıldı",
                "BitmapScreenMessagesController.CreateBitmapScreenMessage",
                new { UserId = userId, Role = userRole });

            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu",
                    "BitmapScreenMessagesController.CreateBitmapScreenMessage",
                    new { ModelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList());

                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var createdMessage = await _bitmapScreenMessageService.CreateBitmapScreenMessageAsync(request);

                await _logService.LogInfoAsync(
                    "Bitmap ekran mesajı oluşturuldu",
                    "BitmapScreenMessagesController.CreateBitmapScreenMessage",
                    new { MessageId = createdMessage.Id, UserId = userId, Role = userRole });

                var response = ApiResponse<BitmapScreenMessageDto>.Created(createdMessage, "Bitmap ekran mesajı başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetBitmapScreenMessageById), new { id = createdMessage.Id }, response);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Bitmap ekran mesajı oluşturulurken hata",
                    "BitmapScreenMessagesController.CreateBitmapScreenMessage",
                    ex,
                    userId,
                    userRole);

                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Bitmap ekran mesajını günceller
        /// </summary>
        /// <param name="id">Bitmap ekran mesaj ID'si</param>
        /// <param name="request">Bitmap ekran mesaj güncelleme isteği</param>
        /// <returns>Başarılı durumda 200 OK ve güncellenen bitmap ekran mesajı, bulunamazsa 404 Not Found, hata durumunda 400 Bad Request</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<BitmapScreenMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdateBitmapScreenMessage(int id, [FromBody] UpdateBitmapScreenMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "UpdateBitmapScreenMessage çağrıldı",
                "BitmapScreenMessagesController.UpdateBitmapScreenMessage",
                new { MessageId = id, UserId = userId, Role = userRole });

            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu",
                    "BitmapScreenMessagesController.UpdateBitmapScreenMessage",
                    new { ModelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });

                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList());

                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var updatedMessage = await _bitmapScreenMessageService.UpdateBitmapScreenMessageAsync(id, request);

                await _logService.LogInfoAsync(
                    "Bitmap ekran mesajı güncellendi",
                    "BitmapScreenMessagesController.UpdateBitmapScreenMessage",
                    new { MessageId = id, UserId = userId, Role = userRole });

                return Ok(ApiResponse<BitmapScreenMessageDto>.Success(updatedMessage, "Bitmap ekran mesajı başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Bitmap ekran mesajı güncellenirken hata",
                    "BitmapScreenMessagesController.UpdateBitmapScreenMessage",
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
        /// Bitmap ekran mesajını siler
        /// </summary>
        /// <param name="id">Bitmap ekran mesaj ID'si</param>
        /// <returns>Başarılı durumda 204 No Content, bulunamazsa 404 Not Found</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<object>), 204)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeleteBitmapScreenMessage(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            await _logService.LogInfoAsync(
                "DeleteBitmapScreenMessage çağrıldı",
                "BitmapScreenMessagesController.DeleteBitmapScreenMessage",
                new { MessageId = id, UserId = userId, Role = userRole });

            try
            {
                await _bitmapScreenMessageService.DeleteBitmapScreenMessageAsync(id);

                await _logService.LogInfoAsync(
                    "Bitmap ekran mesajı silindi",
                    "BitmapScreenMessagesController.DeleteBitmapScreenMessage",
                    new { MessageId = id, UserId = userId, Role = userRole });

                return NoContent();
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Bitmap ekran mesajı silinirken hata",
                    "BitmapScreenMessagesController.DeleteBitmapScreenMessage",
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