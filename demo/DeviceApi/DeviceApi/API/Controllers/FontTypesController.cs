using Core.Utilities;
using DeviceApi.Business.Services.Interfaces;
using Entities.Dtos;
using LogLibrary.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;

namespace DeviceApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FontTypesController : ControllerBase
    {
        private readonly IFontTypeService _fontTypeService;
        private readonly ILogger<FontTypesController> _logger;
        private readonly ILogService _logService;

        public FontTypesController(
            IFontTypeService fontTypeService,
            ILogger<FontTypesController> logger,
            ILogService logService)
        {
            _fontTypeService = fontTypeService;
            _logger = logger;
            _logService = logService;
        }

        /// <summary>
        /// Tüm font türlerini getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<FontTypeDto>>), 200)]
        public async Task<IActionResult> GetAllFontTypes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetAllFontTypes çağrıldı", 
                "FontTypesController.GetAllFontTypes", 
                new { UserId = userId, Role = userRole });
            
            var fontTypes = await _fontTypeService.GetAllFontTypesAsync();
            return Ok(ApiResponse<List<FontTypeDto>>.Success(fontTypes, "Font türleri başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre font türü getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<FontTypeDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetFontTypeById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "GetFontTypeById çağrıldı", 
                "FontTypesController.GetFontTypeById", 
                new { FontTypeId = id, UserId = userId, Role = userRole });
            
            try
            {
                var fontType = await _fontTypeService.GetFontTypeByIdAsync(id);
                return Ok(ApiResponse<FontTypeDto>.Success(fontType, "Font türü başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Font türü bulunamadı", 
                    "FontTypesController.GetFontTypeById", 
                    new { FontTypeId = id, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Key'e göre font türü getirir
        /// </summary>
        [HttpGet("by-key/{key}")]
        [ProducesResponseType(typeof(ApiResponse<FontTypeDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetFontTypeByKey(int key)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            _logger.LogInformation("GetFontTypeByKey çağrıldı: Key: {Key}, UserId: {UserId}, Role: {Role}", 
                key, userId, userRole);
            
            try
            {
                var fontType = await _fontTypeService.GetFontTypeByKeyAsync(key);
                return Ok(ApiResponse<FontTypeDto>.Success(fontType, "Font türü başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogWarningAsync(
                    "Font türü bulunamadı", 
                    "FontTypesController.GetFontTypeByKey", 
                    new { Key = key, UserId = userId, Role = userRole });
                
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Yeni font türü oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<FontTypeDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreateFontType([FromBody] CreateFontTypeRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            await _logService.LogInfoAsync(
                "CreateFontType çağrıldı", 
                "FontTypesController.CreateFontType", 
                new { UserId = userId, Role = userRole });
            
            if (!ModelState.IsValid)
            {
                await _logService.LogWarningAsync(
                    "Geçersiz model durumu", 
                    "FontTypesController.CreateFontType", 
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
                var createdFontType = await _fontTypeService.CreateFontTypeAsync(request);
                
                await _logService.LogInfoAsync(
                    "Font türü oluşturuldu", 
                    "FontTypesController.CreateFontType", 
                    new { FontTypeId = createdFontType.Id, UserId = userId, Role = userRole });
                    
                var response = ApiResponse<FontTypeDto>.Created(createdFontType, "Font türü başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetFontTypeById), new { id = createdFontType.Id }, response);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "Font türü oluşturulurken hata", 
                    "FontTypesController.CreateFontType", 
                    ex,
                    userId,
                    userRole);
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Font türü günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<FontTypeDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdateFontType(int id, [FromBody] UpdateFontTypeRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            _logger.LogInformation("UpdateFontType çağrıldı: FontTypeId: {FontTypeId}, UserId: {UserId}, Role: {Role}", 
                id, userId, userRole);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Geçersiz model durumu: UserId: {UserId}, Role: {Role}", userId, userRole);
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );
                return BadRequest(ApiResponse<object>.Error(errors, "Geçersiz veri"));
            }

            try
            {
                var updatedFontType = await _fontTypeService.UpdateFontTypeAsync(id, request);
                
                _logger.LogInformation("Font türü güncellendi: FontTypeId: {FontTypeId}, UserId: {UserId}, Role: {Role}",
                    id, userId, userRole);
                    
                return Ok(ApiResponse<FontTypeDto>.Success(updatedFontType, "Font türü başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Font türü güncellenirken hata: FontTypeId: {FontTypeId}, UserId: {UserId}, Role: {Role}", 
                    id, userId, userRole);
                    
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Font türü siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeleteFontType(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            _logger.LogInformation("DeleteFontType çağrıldı: FontTypeId: {FontTypeId}, UserId: {UserId}, Role: {Role}", 
                id, userId, userRole);
            
            try
            {
                await _fontTypeService.DeleteFontTypeAsync(id);
                
                _logger.LogInformation("Font türü silindi: FontTypeId: {FontTypeId}, UserId: {UserId}, Role: {Role}",
                    id, userId, userRole);
                    
                return Ok(ApiResponse<object>.Success("Font türü başarıyla silindi"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Font türü silinirken hata: FontTypeId: {FontTypeId}, UserId: {UserId}, Role: {Role}", 
                    id, userId, userRole);
                    
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                else if (ex.Message.Contains("kullanımda"))
                {
                    return BadRequest(ApiResponse<object>.Error(
                        new Dictionary<string, List<string>> { { "FontType", new List<string> { ex.Message } } }, 
                        "İlişkili kayıtlar var"));
                }
                
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
} 