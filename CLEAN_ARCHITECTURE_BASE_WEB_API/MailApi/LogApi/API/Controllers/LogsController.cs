using System;
using System.Threading.Tasks;
using AuthenticationApi.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace deneme.Controllers.LogControllers
{
    [ApiController]
    [Route("api/logs")]
    [Authorize(Roles = "Admin")]
    public class LogsController : ControllerBase
    {
        private readonly ILogRepository _logRepository;
        private readonly IApiLogService _apiLogService;
        
        public LogsController(ILogRepository logRepository, IApiLogService apiLogService)
        {
            _logRepository = logRepository;
            _apiLogService = apiLogService;
        }
        
        #region RequestResponse Logs
        
        /// <summary>
        /// İstek/Yanıt loglarının sayfalanmış listesini getirir
        /// </summary>
        /// <param name="pageNumber">Sayfa numarası (varsayılan: 1)</param>
        /// <param name="pageSize">Sayfa başına kayıt sayısı (varsayılan: 20)</param>
        /// <param name="search">İsteğe bağlı arama terimi</param>
        /// <returns>İstek/Yanıt loglarının sayfalanmış listesi</returns>
        [HttpGet("requests")]
        public async Task<IActionResult> GetRequestResponseLogs(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20,
            [FromQuery] string search = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;
            
            var logs = await _logRepository.GetRequestResponseLogsAsync(pageNumber, pageSize, search);
            
            return Ok(new
            {
                statusCode = 200,
                isSuccess = true,
                data = logs,
                message = "Request/Response logları başarıyla getirildi"
            });
        }
        
        /// <summary>
        /// Belirli bir tarihten daha eski RequestResponse loglarını siler
        /// </summary>
        /// <param name="olderThanDays">Kaç günden eski logların silineceği (null: tarih filtresi yok)</param>
        /// <param name="path">Belirli bir path'e sahip logları silmek için (null: tüm pathler)</param>
        /// <returns>Silinen log sayısı</returns>
        [HttpDelete("requests")]
        public async Task<IActionResult> CleanupRequestLogs(
            [FromQuery] int? olderThanDays = null,
            [FromQuery] string path = null)
        {
            DateTime? olderThan = olderThanDays.HasValue 
                ? DateTime.UtcNow.AddDays(-olderThanDays.Value) 
                : null;
            
            var deletedCount = await _logRepository.DeleteRequestResponseLogsAsync(olderThan, path);
            
            return Ok(new
            {
                statusCode = 200,
                isSuccess = true,
                data = new { deletedCount },
                message = $"{deletedCount} adet istek/yanıt logu silindi"
            });
        }
        
        #endregion
        
        #region API Logs
        
        /// <summary>
        /// API loglarının sayfalanmış listesini getirir
        /// </summary>
        /// <param name="pageNumber">Sayfa numarası (varsayılan: 1)</param>
        /// <param name="pageSize">Sayfa başına kayıt sayısı (varsayılan: 20)</param>
        /// <param name="level">İsteğe bağlı log seviyesi filtresi (Info, Warning, Error)</param>
        /// <param name="search">İsteğe bağlı arama terimi</param>
        /// <returns>API loglarının sayfalanmış listesi</returns>
        [HttpGet("api")]
        public async Task<IActionResult> GetApiLogs(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20,
            [FromQuery] string level = null,
            [FromQuery] string search = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;
            
            var logs = await _logRepository.GetApiLogsAsync(pageNumber, pageSize, level, search);
            
            return Ok(new
            {
                statusCode = 200,
                isSuccess = true,
                data = logs,
                message = "API logları başarıyla getirildi"
            });
        }
        
        /// <summary>
        /// Belirli bir tarihten daha eski API loglarını siler
        /// </summary>
        /// <param name="olderThanDays">Kaç günden eski logların silineceği (null: tarih filtresi yok)</param>
        /// <param name="level">Belirli bir seviyedeki logları silmek için (null: tüm seviyeler)</param>
        /// <returns>Silinen log sayısı</returns>
        [HttpDelete("api")]
        public async Task<IActionResult> CleanupApiLogs(
            [FromQuery] int? olderThanDays = null,
            [FromQuery] string level = null)
        {
            DateTime? olderThan = olderThanDays.HasValue 
                ? DateTime.UtcNow.AddDays(-olderThanDays.Value) 
                : null;
            
            var deletedCount = await _logRepository.DeleteApiLogsAsync(olderThan, level);
            
            return Ok(new
            {
                statusCode = 200,
                isSuccess = true,
                data = new { deletedCount },
                message = $"{deletedCount} adet API logu silindi"
            });
        }
        
        #endregion
        
        #region Cleanup All
        
        /// <summary>
        /// Tüm logları siler
        /// </summary>
        /// <returns>Silinen toplam log sayısı</returns>
        [HttpDelete("all")]
        public async Task<IActionResult> CleanupAllLogs()
        {
            var deletedCount = await _logRepository.DeleteAllLogsAsync();
            
            return Ok(new
            {
                statusCode = 200,
                isSuccess = true,
                data = new { deletedCount },
                message = $"Toplam {deletedCount} adet log silindi"
            });
        }
        
        #endregion
        
        #region Test Logs
        
        /// <summary>
        /// Test amaçlı bir bilgi logu oluşturur
        /// </summary>
        /// <returns>Log oluşturma işlemi sonucu</returns>
        [HttpGet("test")]
        [Authorize] // Herhangi bir yetkilendirilmiş kullanıcı
        public async Task<IActionResult> TestLog()
        {
            await _apiLogService.LogInfoAsync("Test log message from API");
            
            return Ok(new
            {
                statusCode = 200,
                isSuccess = true,
                message = "Test log başarıyla oluşturuldu"
            });
        }
        
        /// <summary>
        /// Test amaçlı bir uyarı logu oluşturur
        /// </summary>
        /// <returns>Log oluşturma işlemi sonucu</returns>
        [HttpGet("test/warning")]
        [Authorize] // Herhangi bir yetkilendirilmiş kullanıcı
        public async Task<IActionResult> TestWarningLog()
        {
            await _apiLogService.LogWarningAsync("Test warning message from API");
            
            return Ok(new
            {
                statusCode = 200,
                isSuccess = true,
                message = "Test warning log başarıyla oluşturuldu"
            });
        }
        
        /// <summary>
        /// Test amaçlı bir hata logu oluşturur
        /// </summary>
        /// <returns>Log oluşturma işlemi sonucu</returns>
        [HttpGet("test/error")]
        [Authorize] // Herhangi bir yetkilendirilmiş kullanıcı
        public async Task<IActionResult> TestErrorLog()
        {
            await _apiLogService.LogErrorAsync("Test error message from API", 
                new System.Exception("This is a test exception"));
            
            return Ok(new
            {
                statusCode = 200,
                isSuccess = true,
                message = "Test error log başarıyla oluşturuldu"
            });
        }
        
        #endregion
    }
} 