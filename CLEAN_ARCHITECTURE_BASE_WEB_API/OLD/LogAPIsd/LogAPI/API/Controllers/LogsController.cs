using System;
using System.Threading.Tasks;
using LogAPI.Business.Services.Logging;
using LogAPI.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LogAPI.API.Controllers
{
    /// <summary>
    /// Log kayıtlarını yönetmek için API kontrolcüsü
    /// </summary>
    [ApiController]
    [Route("api/logs")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class LogsController : ControllerBase
    {
        private readonly ILogRepository _logRepository;
        private readonly ILogger<LogsController> _logger;
        
        /// <summary>
        /// LogsController yapıcısı
        /// </summary>
        /// <param name="logRepository">Log repository</param>
        /// <param name="logger">Logger servisi</param>
        public LogsController(ILogRepository logRepository, ILogger<LogsController> logger)
        {
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        #region RequestResponse Logs
        
        /// <summary>
        /// İstek/Yanıt loglarının sayfalanmış listesini getirir
        /// </summary>
        /// <param name="pageNumber">Sayfa numarası (varsayılan: 1)</param>
        /// <param name="pageSize">Sayfa başına kayıt sayısı (varsayılan: 20, maksimum: 100)</param>
        /// <param name="search">İsteğe bağlı arama terimi</param>
        /// <returns>İstek/Yanıt loglarının sayfalanmış listesi</returns>
        /// <response code="200">İstek/yanıt logları başarıyla getirildi</response>
        /// <response code="401">Kullanıcı kimliği doğrulanmadı</response>
        /// <response code="403">Kullanıcının yeterli yetkisi yok</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpGet("requests")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<RequestResponseLog>>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRequestResponseLogs(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20,
            [FromQuery] string search = null)
        {
            // Giriş değerlerini doğrula
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;
            
            try
            {
                _logger.LogInformation("İstek/yanıt logları getiriliyor: Sayfa={PageNumber}, Boyut={PageSize}, Arama={Search}", 
                    pageNumber, pageSize, search ?? "Yok");
                
                var logs = await _logRepository.GetRequestResponseLogsAsync(pageNumber, pageSize, search);
                
                _logger.LogInformation("İstek/yanıt logları başarıyla getirildi: {Count} öğe", logs.Items != null ? logs.Items.Count() : 0);
                
                return Ok(ApiResponse<PagedResult<RequestResponseLog>>.Success(
                    logs, 
                    "İstek/yanıt logları başarıyla getirildi"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstek/yanıt logları getirilirken hata oluştu");
                return StatusCode(500, ApiResponse<object>.Fail("Loglar getirilirken bir hata oluştu", 500));
            }
        }
        
        /// <summary>
        /// Belirli bir istek/yanıt logunun detaylarını getirir
        /// </summary>
        /// <param name="id">Log ID</param>
        /// <returns>İstek/Yanıt log detayları</returns>
        /// <response code="200">Log başarıyla getirildi</response>
        /// <response code="404">Belirtilen ID ile log bulunamadı</response>
        /// <response code="401">Kullanıcı kimliği doğrulanmadı</response>
        /// <response code="403">Kullanıcının yeterli yetkisi yok</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpGet("requests/{id}")]
        [ProducesResponseType(typeof(ApiResponse<RequestResponseLog>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRequestResponseLogById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(ApiResponse<object>.Fail("Log ID boş olamaz", 400));
            }
            
            try
            {
                _logger.LogInformation("İstek/yanıt logu getiriliyor: ID={LogId}", id);
                
                var log = await _logRepository.GetRequestResponseLogByIdAsync(id);
                
                if (log == null)
                {
                    _logger.LogWarning("İstek/yanıt logu bulunamadı: ID={LogId}", id);
                    return NotFound(ApiResponse<object>.Fail("Log bulunamadı", 404));
                }
                
                _logger.LogInformation("İstek/yanıt logu başarıyla getirildi: ID={LogId}", id);
                
                return Ok(ApiResponse<RequestResponseLog>.Success(
                    log, 
                    "İstek/yanıt logu başarıyla getirildi"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstek/yanıt logu getirilirken hata oluştu: ID={LogId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("Log getirilirken bir hata oluştu", 500));
            }
        }
        
        /// <summary>
        /// Belirli bir tarihten daha eski RequestResponse loglarını siler
        /// </summary>
        /// <param name="olderThanDays">Kaç günden eski logların silineceği (null: tarih filtresi yok)</param>
        /// <param name="path">Belirli bir path'e sahip logları silmek için (null: tüm pathler)</param>
        /// <returns>Silinen log sayısı</returns>
        /// <response code="200">Loglar başarıyla silindi</response>
        /// <response code="401">Kullanıcı kimliği doğrulanmadı</response>
        /// <response code="403">Kullanıcının yeterli yetkisi yok</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpDelete("requests")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CleanupRequestLogs(
            [FromQuery] int? olderThanDays = null,
            [FromQuery] string path = null)
        {
            try
            {
                DateTime? olderThan = olderThanDays.HasValue 
                    ? DateTime.UtcNow.AddDays(-olderThanDays.Value) 
                    : null;
                
                _logger.LogInformation("İstek/yanıt logları siliniyor: OlderThan={OlderThan}, Path={Path}", 
                    olderThan?.ToString("o") ?? "Yok", path ?? "Tüm pathler");
                
                var deletedCount = await _logRepository.DeleteRequestResponseLogsAsync(olderThan, path);
                
                _logger.LogInformation("İstek/yanıt logları başarıyla silindi: {Count} log silindi", deletedCount);
                
                return Ok(ApiResponse<object>.Success(
                    new { deletedCount },
                    $"{deletedCount} adet istek/yanıt logu silindi"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstek/yanıt logları silinirken hata oluştu");
                return StatusCode(500, ApiResponse<object>.Fail("Loglar silinirken bir hata oluştu", 500));
            }
        }
        
        #endregion
        
        #region API Logs
        
        /// <summary>
        /// API loglarının sayfalanmış listesini getirir
        /// </summary>
        /// <param name="pageNumber">Sayfa numarası (varsayılan: 1)</param>
        /// <param name="pageSize">Sayfa başına kayıt sayısı (varsayılan: 20, maksimum: 100)</param>
        /// <param name="level">İsteğe bağlı log seviyesi filtresi (Info, Warning, Error)</param>
        /// <param name="search">İsteğe bağlı arama terimi</param>
        /// <returns>API loglarının sayfalanmış listesi</returns>
        /// <response code="200">API logları başarıyla getirildi</response>
        /// <response code="401">Kullanıcı kimliği doğrulanmadı</response>
        /// <response code="403">Kullanıcının yeterli yetkisi yok</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpGet("api")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ApiLog>>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetApiLogs(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20,
            [FromQuery] string level = null,
            [FromQuery] string search = null)
        {
            // Giriş değerlerini doğrula
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;
            
            try
            {
                _logger.LogInformation("API logları getiriliyor: Sayfa={PageNumber}, Boyut={PageSize}, Seviye={Level}, Arama={Search}", 
                    pageNumber, pageSize, level ?? "Tüm seviyeler", search ?? "Yok");
                
                var logs = await _logRepository.GetApiLogsAsync(pageNumber, pageSize, level, search);
                
                _logger.LogInformation("API logları başarıyla getirildi: {Count} öğe", logs.Items != null ? logs.Items.Count() : 0);
                
                return Ok(ApiResponse<PagedResult<ApiLog>>.Success(
                    logs, 
                    "API logları başarıyla getirildi"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API logları getirilirken hata oluştu");
                return StatusCode(500, ApiResponse<object>.Fail("Loglar getirilirken bir hata oluştu", 500));
            }
        }
        
        /// <summary>
        /// Belirli bir API logunun detaylarını getirir
        /// </summary>
        /// <param name="id">Log ID</param>
        /// <returns>API log detayları</returns>
        /// <response code="200">Log başarıyla getirildi</response>
        /// <response code="404">Belirtilen ID ile log bulunamadı</response>
        /// <response code="401">Kullanıcı kimliği doğrulanmadı</response>
        /// <response code="403">Kullanıcının yeterli yetkisi yok</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpGet("api/{id}")]
        [ProducesResponseType(typeof(ApiResponse<ApiLog>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetApiLogById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(ApiResponse<object>.Fail("Log ID boş olamaz", 400));
            }
            
            try
            {
                _logger.LogInformation("API logu getiriliyor: ID={LogId}", id);
                
                var log = await _logRepository.GetApiLogByIdAsync(id);
                
                if (log == null)
                {
                    _logger.LogWarning("API logu bulunamadı: ID={LogId}", id);
                    return NotFound(ApiResponse<object>.Fail("Log bulunamadı", 404));
                }
                
                _logger.LogInformation("API logu başarıyla getirildi: ID={LogId}", id);
                
                return Ok(ApiResponse<ApiLog>.Success(
                    log, 
                    "API logu başarıyla getirildi"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API logu getirilirken hata oluştu: ID={LogId}", id);
                return StatusCode(500, ApiResponse<object>.Fail("Log getirilirken bir hata oluştu", 500));
            }
        }
        
        /// <summary>
        /// Belirli bir tarihten daha eski API loglarını siler
        /// </summary>
        /// <param name="olderThanDays">Kaç günden eski logların silineceği (null: tarih filtresi yok)</param>
        /// <param name="level">Belirli bir seviyedeki logları silmek için (null: tüm seviyeler)</param>
        /// <returns>Silinen log sayısı</returns>
        /// <response code="200">Loglar başarıyla silindi</response>
        /// <response code="401">Kullanıcı kimliği doğrulanmadı</response>
        /// <response code="403">Kullanıcının yeterli yetkisi yok</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpDelete("api")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CleanupApiLogs(
            [FromQuery] int? olderThanDays = null,
            [FromQuery] string level = null)
        {
            try
            {
                DateTime? olderThan = olderThanDays.HasValue 
                    ? DateTime.UtcNow.AddDays(-olderThanDays.Value) 
                    : null;
                
                _logger.LogInformation("API logları siliniyor: OlderThan={OlderThan}, Level={Level}", 
                    olderThan?.ToString("o") ?? "Yok", level ?? "Tüm seviyeler");
                
                var deletedCount = await _logRepository.DeleteApiLogsAsync(olderThan, level);
                
                _logger.LogInformation("API logları başarıyla silindi: {Count} log silindi", deletedCount);
                
                return Ok(ApiResponse<object>.Success(
                    new { deletedCount },
                    $"{deletedCount} adet API logu silindi"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API logları silinirken hata oluştu");
                return StatusCode(500, ApiResponse<object>.Fail("Loglar silinirken bir hata oluştu", 500));
            }
        }
        
        #endregion
        
        #region Cleanup All
        
        /// <summary>
        /// Tüm logları siler
        /// </summary>
        /// <returns>Silinen toplam log sayısı</returns>
        /// <response code="200">Tüm loglar başarıyla silindi</response>
        /// <response code="401">Kullanıcı kimliği doğrulanmadı</response>
        /// <response code="403">Kullanıcının yeterli yetkisi yok</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpDelete("all")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CleanupAllLogs()
        {
            try
            {
                _logger.LogWarning("TÜM LOGLAR SİLİNİYOR - Bu işlem tüm log verilerini silecektir!");
                
                var deletedCount = await _logRepository.DeleteAllLogsAsync();
                
                _logger.LogInformation("Tüm loglar başarıyla silindi: {Count} log silindi", deletedCount);
                
                return Ok(ApiResponse<object>.Success(
                    new { deletedCount },
                    $"Toplam {deletedCount} adet log silindi"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tüm loglar silinirken hata oluştu");
                return StatusCode(500, ApiResponse<object>.Fail("Loglar silinirken bir hata oluştu", 500));
            }
        }
        
        #endregion
    }
} 