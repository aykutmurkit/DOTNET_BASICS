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
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformService _platformService;
        private readonly ILogService _logService;
        private readonly ILogger<PlatformsController> _logger;

        public PlatformsController(
            IPlatformService platformService,
            ILogService logService,
            ILogger<PlatformsController> logger)
        {
            _platformService = platformService;
            _logService = logService;
            _logger = logger;
        }

        /// <summary>
        /// Tüm platformları cihazları ve tahminleri ile birlikte getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<PlatformDto>>), 200)]
        public async Task<IActionResult> GetAllPlatforms()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetAllPlatforms çağrıldı", 
                "PlatformsController.GetAllPlatforms", 
                new { UserId = userId, Role = userRole });
            
            var platforms = await _platformService.GetAllPlatformsAsync();
            
            return Ok(ApiResponse<List<PlatformDto>>.Success(platforms, "Platformlar başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre platform getirir, cihazları ve tahmini ile birlikte
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PlatformDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetPlatformById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetPlatformById çağrıldı", 
                "PlatformsController.GetPlatformById", 
                new { PlatformId = id, UserId = userId, Role = userRole });
            
            try 
            {
                var platform = await _platformService.GetPlatformByIdAsync(id);
                return Ok(ApiResponse<PlatformDto>.Success(platform, "Platform başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Platform bulunamadı", 
                    "PlatformsController.GetPlatformById", 
                    new { PlatformId = id, Error = ex.Message, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Platform bulunamadı"));
            }
        }

        /// <summary>
        /// İstasyon ID'sine göre platformları getirir
        /// </summary>
        [HttpGet("by-station/{stationId}")]
        [ProducesResponseType(typeof(ApiResponse<List<PlatformDto>>), 200)]
        public async Task<IActionResult> GetPlatformsByStationId(int stationId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetPlatformsByStationId çağrıldı", 
                "PlatformsController.GetPlatformsByStationId", 
                new { StationId = stationId, UserId = userId, Role = userRole });
            
            try 
            {
                var platforms = await _platformService.GetPlatformsByStationIdAsync(stationId);
                return Ok(ApiResponse<List<PlatformDto>>.Success(platforms, $"İstasyon (ID: {stationId}) platformları başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "İstasyon bulunamadı", 
                    "PlatformsController.GetPlatformsByStationId", 
                    new { StationId = stationId, Error = ex.Message, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("İstasyon bulunamadı"));
            }
        }

        /// <summary>
        /// Yeni platform oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<PlatformDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreatePlatform([FromBody] CreatePlatformRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "CreatePlatform çağrıldı", 
                "PlatformsController.CreatePlatform", 
                new { UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "PlatformsController.CreatePlatform", 
                    new { Errors = errors, UserId = userId, Role = userRole });
                
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var createdPlatform = await _platformService.CreatePlatformAsync(request);
                
                await _logService.LogInfoAsync(
                    "Platform oluşturuldu", 
                    "PlatformsController.CreatePlatform", 
                    new { PlatformId = createdPlatform.Id, UserId = userId, Role = userRole });
                
                var response = ApiResponse<PlatformDto>.Created(createdPlatform, "Platform başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetPlatformById), new { id = createdPlatform.Id }, response);
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Platform oluşturulamadı", 
                    "PlatformsController.CreatePlatform", 
                    new { Error = ex.Message, UserId = userId, Role = userRole });
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Platform günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<PlatformDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdatePlatform(int id, [FromBody] UpdatePlatformRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "UpdatePlatform çağrıldı", 
                "PlatformsController.UpdatePlatform", 
                new { PlatformId = id, UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "PlatformsController.UpdatePlatform", 
                    new { PlatformId = id, Errors = errors, UserId = userId, Role = userRole });
                
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var updatedPlatform = await _platformService.UpdatePlatformAsync(id, request);
                
                await _logService.LogInfoAsync(
                    "Platform güncellendi", 
                    "PlatformsController.UpdatePlatform", 
                    new { PlatformId = id, UserId = userId, Role = userRole });
                
                return Ok(ApiResponse<PlatformDto>.Success(updatedPlatform, "Platform başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Platform güncellenemedi", 
                    "PlatformsController.UpdatePlatform", 
                    new { PlatformId = id, Error = ex.Message, UserId = userId, Role = userRole });
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Platform siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeletePlatform(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "DeletePlatform çağrıldı", 
                "PlatformsController.DeletePlatform", 
                new { PlatformId = id, UserId = userId, Role = userRole });
            
            try
            {
                await _platformService.DeletePlatformAsync(id);
                
                await _logService.LogInfoAsync(
                    "Platform silindi", 
                    "PlatformsController.DeletePlatform", 
                    new { PlatformId = id, UserId = userId, Role = userRole });
                
                return Ok(ApiResponse<object>.NoContent("Platform başarıyla silindi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Platform silinemedi", 
                    "PlatformsController.DeletePlatform", 
                    new { PlatformId = id, Error = ex.Message, UserId = userId, Role = userRole });
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
} 