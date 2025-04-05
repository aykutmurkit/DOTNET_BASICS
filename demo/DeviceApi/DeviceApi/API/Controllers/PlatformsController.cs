using Core.Utilities;
using DeviceApi.Business.Services.Interfaces;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformService _platformService;

        public PlatformsController(IPlatformService platformService)
        {
            _platformService = platformService;
        }

        /// <summary>
        /// Tüm platformları getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<PlatformDto>>), 200)]
        public async Task<IActionResult> GetAllPlatforms()
        {
            var platforms = await _platformService.GetAllPlatformsAsync();
            return Ok(new ApiResponse<List<PlatformDto>>
            {
                Data = platforms,
                Message = "Platformlar başarıyla getirildi"
            });
        }

        /// <summary>
        /// ID'ye göre platform getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PlatformDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetPlatformById(int id)
        {
            var platform = await _platformService.GetPlatformByIdAsync(id);
            if (platform == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Platform bulunamadı"
                });
            }
            
            return Ok(new ApiResponse<PlatformDto>
            {
                Data = platform,
                Message = "Platform başarıyla getirildi"
            });
        }

        /// <summary>
        /// İstasyon ID'sine göre platformları getirir
        /// </summary>
        [HttpGet("by-station/{stationId}")]
        [ProducesResponseType(typeof(ApiResponse<List<PlatformDto>>), 200)]
        public async Task<IActionResult> GetPlatformsByStationId(int stationId)
        {
            var platforms = await _platformService.GetPlatformsByStationIdAsync(stationId);
            return Ok(new ApiResponse<List<PlatformDto>>
            {
                Data = platforms,
                Message = $"İstasyon (ID: {stationId}) platformları başarıyla getirildi"
            });
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Geçersiz veri"
                });
            }

            try
            {
                var createdPlatform = await _platformService.CreatePlatformAsync(request);
                return CreatedAtAction(nameof(GetPlatformById), new { id = createdPlatform.Id }, new ApiResponse<PlatformDto>
                {
                    Data = createdPlatform,
                    Message = "Platform başarıyla oluşturuldu"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Geçersiz veri"
                });
            }

            try
            {
                var updatedPlatform = await _platformService.UpdatePlatformAsync(id, request);
                return Ok(new ApiResponse<PlatformDto>
                {
                    Data = updatedPlatform,
                    Message = "Platform başarıyla güncellendi"
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    });
                }
                
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
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
            try
            {
                await _platformService.DeletePlatformAsync(id);
                return Ok(new ApiResponse<object>
                {
                    Message = "Platform başarıyla silindi"
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    });
                }
                
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }
    }
} 