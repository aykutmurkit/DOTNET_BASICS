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
    public class PredictionsController : ControllerBase
    {
        private readonly IPredictionService _predictionService;
        private readonly ILogger<PredictionsController> _logger;
        private readonly ILogService _logService;

        public PredictionsController(
            IPredictionService predictionService,
            ILogger<PredictionsController> logger,
            ILogService logService)
        {
            _predictionService = predictionService;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        /// Tüm tren tahminlerini getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<PredictionDto>>), 200)]
        public async Task<IActionResult> GetAllPredictions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetAllPredictions çağrıldı", 
                "PredictionsController.GetAllPredictions", 
                new { UserId = userId, Role = userRole });
            
            var predictions = await _predictionService.GetAllPredictionsAsync();
            
            return Ok(ApiResponse<List<PredictionDto>>.Success(predictions, "Tren tahminleri başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre tren tahmini getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PredictionDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetPredictionById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetPredictionById çağrıldı", 
                "PredictionsController.GetPredictionById", 
                new { PredictionId = id, UserId = userId, Role = userRole });
            
            try
            {
                var prediction = await _predictionService.GetPredictionByIdAsync(id);
                return Ok(ApiResponse<PredictionDto>.Success(prediction, "Tren tahmini başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Tahmin bulunamadı", 
                    "PredictionsController.GetPredictionById", 
                    new { PredictionId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Platform ID'sine göre tren tahmini getirir
        /// </summary>
        [HttpGet("by-platform/{platformId}")]
        [ProducesResponseType(typeof(ApiResponse<PredictionDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetPredictionByPlatformId(int platformId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetPredictionByPlatformId çağrıldı", 
                "PredictionsController.GetPredictionByPlatformId", 
                new { PlatformId = platformId, UserId = userId, Role = userRole });
            
            try
            {
                var prediction = await _predictionService.GetPredictionByPlatformIdAsync(platformId);
                return Ok(ApiResponse<PredictionDto>.Success(prediction, "Tren tahmini başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "PredictionsController.GetPredictionByPlatformId", 
                    new { PlatformId = platformId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Platform için yeni bir tren tahmini oluşturur
        /// </summary>
        [HttpPost("{platformId}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<PredictionDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> CreatePrediction(int platformId, [FromBody] CreatePredictionRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "CreatePrediction çağrıldı", 
                "PredictionsController.CreatePrediction", 
                new { PlatformId = platformId, UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "PredictionsController.CreatePrediction", 
                    new { PlatformId = platformId, Errors = errors, UserId = userId, Role = userRole });
                
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }
            
            try
            {
                var createdPrediction = await _predictionService.CreatePredictionAsync(platformId, request);
                
                var response = ApiResponse<PredictionDto>.Created(createdPrediction, "Tren tahmini başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetPredictionById), new { id = createdPrediction.Id }, response);
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "PredictionsController.CreatePrediction", 
                    new { PlatformId = platformId, UserId = userId, Role = userRole });
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Mevcut bir tren tahminini günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<PredictionDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdatePrediction(int id, [FromBody] UpdatePredictionRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "UpdatePrediction çağrıldı", 
                "PredictionsController.UpdatePrediction", 
                new { PredictionId = id, UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "PredictionsController.UpdatePrediction", 
                    new { PredictionId = id, Errors = errors, UserId = userId, Role = userRole });
                
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }
            
            try
            {
                var updatedPrediction = await _predictionService.UpdatePredictionAsync(id, request);
                return Ok(ApiResponse<PredictionDto>.Success(updatedPrediction, "Tren tahmini başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "PredictionsController.UpdatePrediction", 
                    new { PredictionId = id, UserId = userId, Role = userRole });
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Tren tahmini siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeletePrediction(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "DeletePrediction çağrıldı", 
                "PredictionsController.DeletePrediction", 
                new { PredictionId = id, UserId = userId, Role = userRole });
            
            try
            {
                await _predictionService.DeletePredictionAsync(id);
                return Ok(ApiResponse<object>.Success("Tren tahmini başarıyla silindi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    ex.Message, 
                    "PredictionsController.DeletePrediction", 
                    new { PredictionId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }
    }
} 