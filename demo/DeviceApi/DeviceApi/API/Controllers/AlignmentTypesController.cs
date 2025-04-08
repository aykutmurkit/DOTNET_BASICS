using Core.Utilities;
using DeviceApi.Business.Services.Interfaces;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeviceApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlignmentTypesController : ControllerBase
    {
        private readonly IAlignmentTypeService _alignmentTypeService;
        private readonly ILogger<AlignmentTypesController> _logger;

        public AlignmentTypesController(IAlignmentTypeService alignmentTypeService, ILogger<AlignmentTypesController> logger)
        {
            _alignmentTypeService = alignmentTypeService;
            _logger = logger;
        }

        /// <summary>
        /// Tüm hizalama türlerini getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<AlignmentTypeDto>>), 200)]
        public async Task<IActionResult> GetAllAlignmentTypes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("GetAllAlignmentTypes çağrıldı: UserId: {UserId}, Role: {Role}", userId, userRole);
            
            var types = await _alignmentTypeService.GetAllAlignmentTypesAsync();
            return Ok(ApiResponse<List<AlignmentTypeDto>>.Success(types, "Hizalama türleri başarıyla getirildi"));
        }

        /// <summary>
        /// ID'ye göre hizalama türü getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AlignmentTypeDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetAlignmentTypeById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("GetAlignmentTypeById çağrıldı: ID: {Id}, UserId: {UserId}, Role: {Role}", id, userId, userRole);
            
            try
            {
                var type = await _alignmentTypeService.GetAlignmentTypeByIdAsync(id);
                return Ok(ApiResponse<AlignmentTypeDto>.Success(type, "Hizalama türü başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Key değerine göre hizalama türü getirir
        /// </summary>
        [HttpGet("by-key/{key}")]
        [ProducesResponseType(typeof(ApiResponse<AlignmentTypeDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetAlignmentTypeByKey(int key)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("GetAlignmentTypeByKey çağrıldı: Key: {Key}, UserId: {UserId}, Role: {Role}", key, userId, userRole);
            
            try
            {
                var type = await _alignmentTypeService.GetAlignmentTypeByKeyAsync(key);
                return Ok(ApiResponse<AlignmentTypeDto>.Success(type, "Hizalama türü başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }

        /// <summary>
        /// Yeni hizalama türü oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<AlignmentTypeDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreateAlignmentType([FromBody] CreateAlignmentTypeRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("CreateAlignmentType çağrıldı: UserId: {UserId}, Role: {Role}", userId, userRole);
            
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
                var createdType = await _alignmentTypeService.CreateAlignmentTypeAsync(request);
                var response = ApiResponse<AlignmentTypeDto>.Created(createdType, "Hizalama türü başarıyla oluşturuldu");
                return CreatedAtAction(nameof(GetAlignmentTypeById), new { id = createdType.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Hizalama türü günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<AlignmentTypeDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdateAlignmentType(int id, [FromBody] UpdateAlignmentTypeRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("UpdateAlignmentType çağrıldı: ID: {Id}, UserId: {UserId}, Role: {Role}", id, userId, userRole);
            
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
                var updatedType = await _alignmentTypeService.UpdateAlignmentTypeAsync(id, request);
                return Ok(ApiResponse<AlignmentTypeDto>.Success(updatedType, "Hizalama türü başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Hizalama türü siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeleteAlignmentType(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("DeleteAlignmentType çağrıldı: ID: {Id}, UserId: {UserId}, Role: {Role}", id, userId, userRole);
            
            try
            {
                await _alignmentTypeService.DeleteAlignmentTypeAsync(id);
                return Ok(ApiResponse<object>.Success("Hizalama türü başarıyla silindi"));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
} 