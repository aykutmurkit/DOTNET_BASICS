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
    public class StationsController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationsController(IStationService stationService)
        {
            _stationService = stationService;
        }

        /// <summary>
        /// Tüm istasyonları sadece temel bilgilerle getirir
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<StationDto>>), 200)]
        public async Task<IActionResult> GetAllStations()
        {
            var stations = await _stationService.GetStationsBasicAsync();
            return Ok(new ApiResponse<List<StationDto>>
            {
                Data = stations,
                Message = "İstasyonlar başarıyla getirildi"
            });
        }
        
        /// <summary>
        /// Tüm istasyonları platformları ile birlikte getirir (cihazlar olmadan)
        /// </summary>
        [HttpGet("with-platforms")]
        [ProducesResponseType(typeof(ApiResponse<List<StationDto>>), 200)]
        public async Task<IActionResult> GetStationsWithPlatforms()
        {
            var stations = await _stationService.GetStationsWithPlatformsAsync();
            return Ok(new ApiResponse<List<StationDto>>
            {
                Data = stations,
                Message = "İstasyonlar ve platformları başarıyla getirildi"
            });
        }
        
        /// <summary>
        /// Tüm istasyonları platformları ve cihazları ile birlikte getirir
        /// </summary>
        [HttpGet("with-platforms-devices")]
        [ProducesResponseType(typeof(ApiResponse<List<StationDto>>), 200)]
        public async Task<IActionResult> GetStationsWithPlatformsAndDevices()
        {
            var stations = await _stationService.GetStationsWithPlatformsAndDevicesAsync();
            return Ok(new ApiResponse<List<StationDto>>
            {
                Data = stations,
                Message = "İstasyonlar, platformları ve cihazları başarıyla getirildi"
            });
        }

        /// <summary>
        /// ID'ye göre istasyon getirir
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StationDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> GetStationById(int id)
        {
            var station = await _stationService.GetStationByIdAsync(id);
            if (station == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "İstasyon bulunamadı"
                });
            }
            
            return Ok(new ApiResponse<StationDto>
            {
                Data = station,
                Message = "İstasyon başarıyla getirildi"
            });
        }

        /// <summary>
        /// Yeni istasyon oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<StationDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> CreateStation([FromBody] CreateStationRequest request)
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
                var createdStation = await _stationService.CreateStationAsync(request);
                return CreatedAtAction(nameof(GetStationById), new { id = createdStation.Id }, new ApiResponse<StationDto>
                {
                    Data = createdStation,
                    Message = "İstasyon başarıyla oluşturuldu"
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
        /// İstasyon günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<StationDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> UpdateStation(int id, [FromBody] UpdateStationRequest request)
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
                var updatedStation = await _stationService.UpdateStationAsync(id, request);
                return Ok(new ApiResponse<StationDto>
                {
                    Data = updatedStation,
                    Message = "İstasyon başarıyla güncellendi"
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
        /// İstasyon siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 404)]
        public async Task<IActionResult> DeleteStation(int id)
        {
            try
            {
                await _stationService.DeleteStationAsync(id);
                return Ok(new ApiResponse<object>
                {
                    Message = "İstasyon başarıyla silindi"
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