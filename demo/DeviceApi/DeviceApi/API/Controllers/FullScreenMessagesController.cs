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
    public class FullScreenMessagesController : ControllerBase
    {
        private readonly IFullScreenMessageService _fullScreenMessageService;
        private readonly ILogger<FullScreenMessagesController> _logger;
        private readonly ILogService _logService;

        public FullScreenMessagesController(
            IFullScreenMessageService fullScreenMessageService,
            ILogger<FullScreenMessagesController> logger,
            ILogService logService)
        {
            _fullScreenMessageService = fullScreenMessageService;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        /// Tüm tam ekran mesajları getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<FullScreenMessageDto>>), 200)]
        public async Task<IActionResult> GetAllFullScreenMessages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetAllFullScreenMessages çağrıldı", 
                "FullScreenMessagesController.GetAllFullScreenMessages", 
                new { UserId = userId, Role = userRole });
            
            var messages = await _fullScreenMessageService.GetAllFullScreenMessagesAsync();
            
            return Ok(ApiResponse<List<FullScreenMessageDto>>.Success(messages, "Tam ekran mesajlar başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre tam ekran mesaj getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<FullScreenMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetFullScreenMessageById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetFullScreenMessageById çağrıldı", 
                "FullScreenMessagesController.GetFullScreenMessageById", 
                new { MessageId = id, UserId = userId, Role = userRole });
            
            try
            {
                var message = await _fullScreenMessageService.GetFullScreenMessageByIdAsync(id);
                return Ok(ApiResponse<FullScreenMessageDto>.Success(message, "Tam ekran mesaj başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Tam ekran mesaj bulunamadı", 
                    "FullScreenMessagesController.GetFullScreenMessageById", 
                    new { MessageId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Cihaz ID'sine göre tam ekran mesaj getirir
        /// </summary>
        [HttpGet("by-device/{deviceId}")]
        [ProducesResponseType(typeof(ApiResponse<FullScreenMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetFullScreenMessageByDeviceId(int deviceId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetFullScreenMessageByDeviceId çağrıldı", 
                "FullScreenMessagesController.GetFullScreenMessageByDeviceId", 
                new { DeviceId = deviceId, UserId = userId, Role = userRole });
            
            try
            {
                var message = await _fullScreenMessageService.GetFullScreenMessageByDeviceIdAsync(deviceId);
                return Ok(ApiResponse<FullScreenMessageDto>.Success(message, "Tam ekran mesaj başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "FullScreenMessagesController.GetFullScreenMessageByDeviceId", 
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }
        
        /// <summary>
        /// Bir mesaja bağlı cihaz ID'lerini getirir
        /// </summary>
        [HttpGet("{id}/devices")]
        [ProducesResponseType(typeof(ApiResponse<List<int>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetDevicesByFullScreenMessageId(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetDevicesByFullScreenMessageId çağrıldı", 
                "FullScreenMessagesController.GetDevicesByFullScreenMessageId", 
                new { MessageId = id, UserId = userId, Role = userRole });
            
            try
            {
                var deviceIds = await _fullScreenMessageService.GetDeviceIdsByFullScreenMessageIdAsync(id);
                return Ok(ApiResponse<List<int>>.Success(deviceIds, "Cihaz ID'leri başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "FullScreenMessagesController.GetDevicesByFullScreenMessageId", 
                    new { MessageId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Yeni bir tam ekran mesaj oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<FullScreenMessageDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreateFullScreenMessage([FromBody] CreateFullScreenMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "CreateFullScreenMessage çağrıldı", 
                "FullScreenMessagesController.CreateFullScreenMessage", 
                new { UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "FullScreenMessagesController.CreateFullScreenMessage", 
                    new { Errors = errors, UserId = userId, Role = userRole });
                
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }
            
            try
            {
                var createdMessage = await _fullScreenMessageService.CreateFullScreenMessageAsync(request);
                
                var response = ApiResponse<FullScreenMessageDto>.Created(createdMessage, "Tam ekran mesaj başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetFullScreenMessageById), new { id = createdMessage.Id }, response);
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "FullScreenMessagesController.CreateFullScreenMessage", 
                    new { UserId = userId, Role = userRole });
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
        
        /// <summary>
        /// Cihaza mesaj atar (atama yapar)
        /// </summary>
        [HttpPost("assign")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> AssignMessageToDevice([FromBody] AssignFullScreenMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "AssignMessageToDevice çağrıldı", 
                "FullScreenMessagesController.AssignMessageToDevice", 
                new { DeviceId = request.DeviceId, MessageId = request.FullScreenMessageId, UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }
            
            try
            {
                await _fullScreenMessageService.AssignMessageToDeviceAsync(request);
                return Ok(ApiResponse<object>.Success("Tam ekran mesaj cihaza başarıyla atandı"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "FullScreenMessagesController.AssignMessageToDevice", 
                    new { DeviceId = request.DeviceId, MessageId = request.FullScreenMessageId, UserId = userId, Role = userRole });
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
        
        /// <summary>
        /// Cihazdan mesajı kaldırır
        /// </summary>
        [HttpDelete("unassign/{deviceId}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UnassignMessageFromDevice(int deviceId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "UnassignMessageFromDevice çağrıldı", 
                "FullScreenMessagesController.UnassignMessageFromDevice", 
                new { DeviceId = deviceId, UserId = userId, Role = userRole });
            
            try
            {
                await _fullScreenMessageService.UnassignMessageFromDeviceAsync(deviceId);
                return Ok(ApiResponse<object>.Success("Tam ekran mesaj cihazdan başarıyla kaldırıldı"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "FullScreenMessagesController.UnassignMessageFromDevice", 
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Mevcut bir tam ekran mesajı günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<FullScreenMessageDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdateFullScreenMessage(int id, [FromBody] UpdateFullScreenMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "UpdateFullScreenMessage çağrıldı", 
                "FullScreenMessagesController.UpdateFullScreenMessage", 
                new { MessageId = id, UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "FullScreenMessagesController.UpdateFullScreenMessage", 
                    new { MessageId = id, Errors = errors, UserId = userId, Role = userRole });
                
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }
            
            try
            {
                var updatedMessage = await _fullScreenMessageService.UpdateFullScreenMessageAsync(id, request);
                return Ok(ApiResponse<FullScreenMessageDto>.Success(updatedMessage, "Tam ekran mesaj başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "FullScreenMessagesController.UpdateFullScreenMessage", 
                    new { MessageId = id, UserId = userId, Role = userRole });
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Tam ekran mesaj siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeleteFullScreenMessage(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "DeleteFullScreenMessage çağrıldı", 
                "FullScreenMessagesController.DeleteFullScreenMessage", 
                new { MessageId = id, UserId = userId, Role = userRole });
            
            try
            {
                await _fullScreenMessageService.DeleteFullScreenMessageAsync(id);
                return Ok(ApiResponse<object>.Success("Tam ekran mesaj başarıyla silindi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "FullScreenMessagesController.DeleteFullScreenMessage", 
                    new { MessageId = id, UserId = userId, Role = userRole });
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
} 