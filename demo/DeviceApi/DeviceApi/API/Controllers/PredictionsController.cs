using Core.Utilities;
using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
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
    public class PredictionsController : ControllerBase
    {
        private readonly IPredictionRepository _predictionRepository;
        private readonly IPlatformRepository _platformRepository;
        private readonly ILogger<PredictionsController> _logger;
        private readonly ILogService _logService;

        public PredictionsController(
            IPredictionRepository predictionRepository,
            IPlatformRepository platformRepository,
            ILogger<PredictionsController> logger,
            ILogService logService)
        {
            _predictionRepository = predictionRepository;
            _platformRepository = platformRepository;
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
            
            var predictions = await _predictionRepository.GetAllPredictionsAsync();
            var predictionDtos = predictions.Select(MapToPredictionDto).ToList();
            
            return Ok(ApiResponse<List<PredictionDto>>.Success(predictionDtos, "Tren tahminleri başarıyla getirildi"));
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
            
            var prediction = await _predictionRepository.GetPredictionByIdAsync(id);
            if (prediction == null)
            {
                await _logService.LogWarningAsync(
                    "Tahmin bulunamadı", 
                    "PredictionsController.GetPredictionById", 
                    new { PredictionId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Tren tahmini bulunamadı"));
            }
            
            var predictionDto = MapToPredictionDto(prediction);
            return Ok(ApiResponse<PredictionDto>.Success(predictionDto, "Tren tahmini başarıyla getirildi"));
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
            
            // Önce platformun var olduğunu kontrol et
            var platform = await _platformRepository.GetPlatformByIdAsync(platformId);
            if (platform == null)
            {
                await _logService.LogWarningAsync(
                    "Platform bulunamadı", 
                    "PredictionsController.GetPredictionByPlatformId", 
                    new { PlatformId = platformId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Platform bulunamadı"));
            }
            
            var prediction = await _predictionRepository.GetPredictionByPlatformIdAsync(platformId);
            if (prediction == null)
            {
                await _logService.LogWarningAsync(
                    "Platform için tahmin bulunamadı", 
                    "PredictionsController.GetPredictionByPlatformId", 
                    new { PlatformId = platformId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Platform için tren tahmini bulunamadı"));
            }
            
            var predictionDto = MapToPredictionDto(prediction);
            return Ok(ApiResponse<PredictionDto>.Success(predictionDto, "Tren tahmini başarıyla getirildi"));
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
            
            // Platform var mı kontrol et
            var platform = await _platformRepository.GetPlatformByIdAsync(platformId);
            if (platform == null)
            {
                await _logService.LogWarningAsync(
                    "Platform bulunamadı", 
                    "PredictionsController.CreatePrediction", 
                    new { PlatformId = platformId, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Platform bulunamadı"));
            }
            
            // Platform için zaten bir tahmin var mı kontrol et
            var existingPrediction = await _predictionRepository.GetPredictionByPlatformIdAsync(platformId);
            if (existingPrediction != null)
            {
                await _logService.LogWarningAsync(
                    "Platform için zaten bir tahmin mevcut", 
                    "PredictionsController.CreatePrediction", 
                    new { PlatformId = platformId, ExistingPredictionId = existingPrediction.Id, UserId = userId, Role = userRole });
                
                return BadRequest(ApiResponse<object>.Error("Bu platform için zaten bir tren tahmini mevcut. Güncelleme yapabilirsiniz."));
            }
            
            // Yeni tahmin oluştur
            var prediction = new Prediction
            {
                StationName = request.StationName,
                Direction = request.Direction,
                Train1 = request.Train1,
                Line1 = request.Line1,
                Destination1 = request.Destination1,
                Time1 = request.Time1,
                Train2 = request.Train2,
                Line2 = request.Line2,
                Destination2 = request.Destination2,
                Time2 = request.Time2,
                Train3 = request.Train3,
                Line3 = request.Line3,
                Destination3 = request.Destination3,
                Time3 = request.Time3,
                ForecastGenerationAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                PlatformId = platformId
            };
            
            await _predictionRepository.AddPredictionAsync(prediction);
            
            await _logService.LogInfoAsync(
                "Tren tahmini oluşturuldu", 
                "PredictionsController.CreatePrediction", 
                new { PredictionId = prediction.Id, PlatformId = platformId, UserId = userId, Role = userRole });
            
            var predictionDto = MapToPredictionDto(prediction);
            var response = ApiResponse<PredictionDto>.Created(predictionDto, "Tren tahmini başarıyla oluşturuldu");
            
            return CreatedAtAction(nameof(GetPredictionById), new { id = prediction.Id }, response);
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
            
            // Tahmin var mı kontrol et
            var existingPrediction = await _predictionRepository.GetPredictionByIdAsync(id);
            if (existingPrediction == null)
            {
                await _logService.LogWarningAsync(
                    "Tahmin bulunamadı", 
                    "PredictionsController.UpdatePrediction", 
                    new { PredictionId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Tren tahmini bulunamadı"));
            }
            
            // Tahmini güncelle
            existingPrediction.StationName = request.StationName;
            existingPrediction.Direction = request.Direction;
            existingPrediction.Train1 = request.Train1;
            existingPrediction.Line1 = request.Line1;
            existingPrediction.Destination1 = request.Destination1;
            existingPrediction.Time1 = request.Time1;
            existingPrediction.Train2 = request.Train2;
            existingPrediction.Line2 = request.Line2;
            existingPrediction.Destination2 = request.Destination2;
            existingPrediction.Time2 = request.Time2;
            existingPrediction.Train3 = request.Train3;
            existingPrediction.Line3 = request.Line3;
            existingPrediction.Destination3 = request.Destination3;
            existingPrediction.Time3 = request.Time3;
            existingPrediction.ForecastGenerationAt = DateTime.Now;
            
            await _predictionRepository.UpdatePredictionAsync(existingPrediction);
            
            await _logService.LogInfoAsync(
                "Tren tahmini güncellendi", 
                "PredictionsController.UpdatePrediction", 
                new { PredictionId = id, UserId = userId, Role = userRole });
            
            var predictionDto = MapToPredictionDto(existingPrediction);
            return Ok(ApiResponse<PredictionDto>.Success(predictionDto, "Tren tahmini başarıyla güncellendi"));
        }

        /// <summary>
        /// Bir tren tahminini siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
            
            var prediction = await _predictionRepository.GetPredictionByIdAsync(id);
            if (prediction == null)
            {
                await _logService.LogWarningAsync(
                    "Tahmin bulunamadı", 
                    "PredictionsController.DeletePrediction", 
                    new { PredictionId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound("Tren tahmini bulunamadı"));
            }
            
            await _predictionRepository.DeletePredictionAsync(id);
            
            await _logService.LogInfoAsync(
                "Tren tahmini silindi", 
                "PredictionsController.DeletePrediction", 
                new { PredictionId = id, UserId = userId, Role = userRole });
            
            return Ok(ApiResponse<object>.NoContent("Tren tahmini başarıyla silindi"));
        }

        /// <summary>
        /// Prediction nesnesini PredictionDto'ya dönüştürür
        /// </summary>
        private PredictionDto MapToPredictionDto(Prediction prediction)
        {
            return new PredictionDto
            {
                Id = prediction.Id,
                StationName = prediction.StationName,
                Direction = prediction.Direction,
                Train1 = prediction.Train1,
                Line1 = prediction.Line1,
                Destination1 = prediction.Destination1,
                Time1 = prediction.Time1,
                Train2 = prediction.Train2,
                Line2 = prediction.Line2,
                Destination2 = prediction.Destination2,
                Time2 = prediction.Time2,
                Train3 = prediction.Train3,
                Line3 = prediction.Line3,
                Destination3 = prediction.Destination3,
                Time3 = prediction.Time3,
                ForecastGenerationAt = prediction.ForecastGenerationAt,
                CreatedAt = prediction.CreatedAt,
                PlatformId = prediction.PlatformId
            };
        }
    }
} 