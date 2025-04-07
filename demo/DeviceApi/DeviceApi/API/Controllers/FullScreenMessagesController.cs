using Core.Utilities;
using Data.Interfaces;
using Entities.Concrete;
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
        private readonly IFullScreenMessageRepository _fullScreenMessageRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILogger<FullScreenMessagesController> _logger;
        private readonly ILogService _logService;

        public FullScreenMessagesController(
            IFullScreenMessageRepository fullScreenMessageRepository,
            IDeviceRepository deviceRepository,
            ILogger<FullScreenMessagesController> logger,
            ILogService logService)
        {
            _fullScreenMessageRepository = fullScreenMessageRepository;
            _deviceRepository = deviceRepository;
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
            
            var messages = await _fullScreenMessageRepository.GetAllFullScreenMessagesAsync();
            var messageDtos = messages.Select(MapToFullScreenMessageDto).ToList();
            
            return Ok(ApiResponse<List<FullScreenMessageDto>>.Success(messageDtos, "Tam ekran mesajlar başarıyla getirildi"));
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
            
            var message = await _fullScreenMessageRepository.GetFullScreenMessageByIdAsync(id);
            if (message == null)
            {
                await _logService.LogWarningAsync(
                    "Tam ekran mesaj bulunamadı", 
                    "FullScreenMessagesController.GetFullScreenMessageById", 
                    new { MessageId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Tam ekran mesaj bulunamadı"));
            }
            
            var messageDto = MapToFullScreenMessageDto(message);
            return Ok(ApiResponse<FullScreenMessageDto>.Success(messageDto, "Tam ekran mesaj başarıyla getirildi"));
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
            
            // Önce cihazın var olduğunu kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                await _logService.LogWarningAsync(
                    "Cihaz bulunamadı", 
                    "FullScreenMessagesController.GetFullScreenMessageByDeviceId", 
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Cihaz bulunamadı"));
            }
            
            var message = await _fullScreenMessageRepository.GetFullScreenMessageByDeviceIdAsync(deviceId);
            if (message == null)
            {
                await _logService.LogWarningAsync(
                    "Cihaz için tam ekran mesaj bulunamadı", 
                    "FullScreenMessagesController.GetFullScreenMessageByDeviceId", 
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Cihaz için tam ekran mesaj bulunamadı"));
            }
            
            var messageDto = MapToFullScreenMessageDto(message);
            return Ok(ApiResponse<FullScreenMessageDto>.Success(messageDto, "Tam ekran mesaj başarıyla getirildi"));
        }

        /// <summary>
        /// Cihaz için yeni bir tam ekran mesaj oluşturur
        /// </summary>
        [HttpPost("{deviceId}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<FullScreenMessageDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> CreateFullScreenMessage(int deviceId, [FromBody] CreateFullScreenMessageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "CreateFullScreenMessage çağrıldı", 
                "FullScreenMessagesController.CreateFullScreenMessage", 
                new { DeviceId = deviceId, UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "FullScreenMessagesController.CreateFullScreenMessage", 
                    new { DeviceId = deviceId, Errors = errors, UserId = userId, Role = userRole });
                
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }
            
            // Cihaz var mı kontrol et
            var device = await _deviceRepository.GetDeviceByIdAsync(deviceId);
            if (device == null)
            {
                await _logService.LogWarningAsync(
                    "Cihaz bulunamadı", 
                    "FullScreenMessagesController.CreateFullScreenMessage", 
                    new { DeviceId = deviceId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Cihaz bulunamadı"));
            }
            
            // Cihaz için zaten bir mesaj var mı kontrol et
            var existingMessage = await _fullScreenMessageRepository.GetFullScreenMessageByDeviceIdAsync(deviceId);
            if (existingMessage != null)
            {
                await _logService.LogWarningAsync(
                    "Cihaz için zaten bir tam ekran mesaj mevcut", 
                    "FullScreenMessagesController.CreateFullScreenMessage", 
                    new { DeviceId = deviceId, ExistingMessageId = existingMessage.Id, UserId = userId, Role = userRole });
                
                return BadRequest(ApiResponse<object>.Error("Bu cihaz için zaten bir tam ekran mesaj mevcut. Güncelleme yapabilirsiniz."));
            }
            
            // Yeni mesaj oluştur
            var message = new FullScreenMessage
            {
                TurkishLine1 = request.TurkishLine1,
                TurkishLine2 = request.TurkishLine2,
                TurkishLine3 = request.TurkishLine3,
                TurkishLine4 = request.TurkishLine4,
                EnglishLine1 = request.EnglishLine1,
                EnglishLine2 = request.EnglishLine2,
                EnglishLine3 = request.EnglishLine3,
                EnglishLine4 = request.EnglishLine4,
                CreatedAt = DateTime.Now,
                DeviceId = deviceId
            };
            
            await _fullScreenMessageRepository.AddFullScreenMessageAsync(message);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj oluşturuldu", 
                "FullScreenMessagesController.CreateFullScreenMessage", 
                new { MessageId = message.Id, DeviceId = deviceId, UserId = userId, Role = userRole });
            
            var messageDto = MapToFullScreenMessageDto(message);
            var response = ApiResponse<FullScreenMessageDto>.Created(messageDto, "Tam ekran mesaj başarıyla oluşturuldu");
            
            return CreatedAtAction(nameof(GetFullScreenMessageById), new { id = message.Id }, response);
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
            
            // Mesaj var mı kontrol et
            var existingMessage = await _fullScreenMessageRepository.GetFullScreenMessageByIdAsync(id);
            if (existingMessage == null)
            {
                await _logService.LogWarningAsync(
                    "Tam ekran mesaj bulunamadı", 
                    "FullScreenMessagesController.UpdateFullScreenMessage", 
                    new { MessageId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Tam ekran mesaj bulunamadı"));
            }
            
            // Mesajı güncelle
            existingMessage.TurkishLine1 = request.TurkishLine1;
            existingMessage.TurkishLine2 = request.TurkishLine2;
            existingMessage.TurkishLine3 = request.TurkishLine3;
            existingMessage.TurkishLine4 = request.TurkishLine4;
            existingMessage.EnglishLine1 = request.EnglishLine1;
            existingMessage.EnglishLine2 = request.EnglishLine2;
            existingMessage.EnglishLine3 = request.EnglishLine3;
            existingMessage.EnglishLine4 = request.EnglishLine4;
            existingMessage.ModifiedAt = DateTime.Now;
            
            await _fullScreenMessageRepository.UpdateFullScreenMessageAsync(existingMessage);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj güncellendi", 
                "FullScreenMessagesController.UpdateFullScreenMessage", 
                new { MessageId = id, UserId = userId, Role = userRole });
            
            var messageDto = MapToFullScreenMessageDto(existingMessage);
            return Ok(ApiResponse<FullScreenMessageDto>.Success(messageDto, "Tam ekran mesaj başarıyla güncellendi"));
        }

        /// <summary>
        /// Bir tam ekran mesajı siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
            
            var message = await _fullScreenMessageRepository.GetFullScreenMessageByIdAsync(id);
            if (message == null)
            {
                await _logService.LogWarningAsync(
                    "Tam ekran mesaj bulunamadı", 
                    "FullScreenMessagesController.DeleteFullScreenMessage", 
                    new { MessageId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Tam ekran mesaj bulunamadı"));
            }
            
            await _fullScreenMessageRepository.DeleteFullScreenMessageAsync(id);
            
            await _logService.LogInfoAsync(
                "Tam ekran mesaj silindi", 
                "FullScreenMessagesController.DeleteFullScreenMessage", 
                new { MessageId = id, UserId = userId, Role = userRole });
            
            return Ok(ApiResponse<object>.NoContent("Tam ekran mesaj başarıyla silindi"));
        }

        /// <summary>
        /// FullScreenMessage nesnesini FullScreenMessageDto'ya dönüştürür
        /// </summary>
        private FullScreenMessageDto MapToFullScreenMessageDto(FullScreenMessage message)
        {
            return new FullScreenMessageDto
            {
                Id = message.Id,
                TurkishLine1 = message.TurkishLine1,
                TurkishLine2 = message.TurkishLine2,
                TurkishLine3 = message.TurkishLine3,
                TurkishLine4 = message.TurkishLine4,
                EnglishLine1 = message.EnglishLine1,
                EnglishLine2 = message.EnglishLine2,
                EnglishLine3 = message.EnglishLine3,
                EnglishLine4 = message.EnglishLine4,
                CreatedAt = message.CreatedAt,
                ModifiedAt = message.ModifiedAt,
                DeviceId = message.DeviceId
            };
        }
    }
} 