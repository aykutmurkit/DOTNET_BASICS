using LogLibrary.Configuration.Settings;
using LogLibrary.Core.Interfaces;
using LogLibrary.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace LogLibrary.Services.Concrete
{
    /// <summary>
    /// Log servisi implementasyonu
    /// </summary>
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly LogSettings _logSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LogService> _logger;
        private readonly GraylogService _graylogService;

        /// <summary>
        /// LogService constructor
        /// </summary>
        public LogService(
            ILogRepository logRepository,
            IOptions<LogSettings> logSettingsOptions,
            IHttpContextAccessor httpContextAccessor,
            ILogger<LogService> logger,
            GraylogService graylogService)
        {
            _logRepository = logRepository;
            _logSettings = logSettingsOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _graylogService = graylogService;
        }

        /// <summary>
        /// Bilgi seviyesinde log kaydı oluşturur
        /// </summary>
        public async Task LogInfoAsync(string message, string source, object data = null, string userId = null, string userName = null, string userEmail = null)
        {
            await CreateLogAsync("Info", message, source, data, null, userId, userName, userEmail);
            _logger.LogInformation($"[{source}] {message}");
        }

        /// <summary>
        /// Uyarı seviyesinde log kaydı oluşturur
        /// </summary>
        public async Task LogWarningAsync(string message, string source, object data = null, string userId = null, string userName = null, string userEmail = null)
        {
            await CreateLogAsync("Warning", message, source, data, null, userId, userName, userEmail);
            _logger.LogWarning($"[{source}] {message}");
        }

        /// <summary>
        /// Hata seviyesinde log kaydı oluşturur
        /// </summary>
        public async Task LogErrorAsync(string message, string source, Exception exception = null, string userId = null, string userName = null, string userEmail = null)
        {
            await CreateLogAsync("Error", message, source, exception, exception, userId, userName, userEmail);
            _logger.LogError(exception, $"[{source}] {message}");
        }

        /// <summary>
        /// Debug seviyesinde log kaydı oluşturur
        /// </summary>
        public async Task LogDebugAsync(string message, string source, object data = null, string userId = null, string userName = null, string userEmail = null)
        {
            await CreateLogAsync("Debug", message, source, data, null, userId, userName, userEmail);
            _logger.LogDebug($"[{source}] {message}");
        }

        /// <summary>
        /// Kritik seviyesinde log kaydı oluşturur
        /// </summary>
        public async Task LogCriticalAsync(string message, string source, Exception exception = null, string userId = null, string userName = null, string userEmail = null)
        {
            await CreateLogAsync("Critical", message, source, exception, exception, userId, userName, userEmail);
            _logger.LogCritical(exception, $"[{source}] {message}");
        }

        /// <summary>
        /// Belirtilen tarihten daha eski log kayıtlarını siler
        /// </summary>
        public async Task<long> CleanupLogsAsync(DateTime olderThan)
        {
            return await _logRepository.DeleteOldLogsAsync(olderThan);
        }

        /// <summary>
        /// Log kayıtlarını sayfalı olarak alır
        /// </summary>
        public async Task<(IEnumerable<LogEntry> Items, long TotalCount)> GetLogsAsync(
            int pageNumber, 
            int pageSize, 
            string level = null, 
            string searchTerm = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            return await _logRepository.GetLogsAsync(pageNumber, pageSize, level, searchTerm, startDate, endDate);
        }

        /// <summary>
        /// HTTP istek ve yanıtları için log kaydı oluşturur
        /// </summary>
        public async Task LogHttpAsync(
            string path,
            string method,
            int? statusCode,
            long? durationMs,
            string traceId,
            string userId = null,
            string userName = null,
            string ipAddress = null,
            object requestData = null,
            object responseData = null)
        {
            if (!_logSettings.EnableHttpLogging)
                return;

            try
            {
                var logEntry = new LogEntry
                {
                    Level = statusCode.HasValue && statusCode.Value >= 400 ? "Warning" : "Info",
                    Message = $"HTTP {method} {path} - {statusCode} ({durationMs}ms)",
                    Source = "HttpRequest",
                    Timestamp = DateTime.UtcNow,
                    TraceId = traceId,
                    UserId = userId,
                    UserName = userName,
                    IpAddress = ipAddress,
                    ApplicationName = _logSettings.ApplicationName,
                    Environment = _logSettings.Environment,
                    HttpPath = path,
                    HttpMethod = method,
                    HttpStatusCode = statusCode,
                    HttpDurationMs = durationMs,
                    Request = _logSettings.MaskSensitiveData ? MaskSensitiveData(requestData) : requestData,
                    Response = _logSettings.MaskSensitiveData ? MaskSensitiveData(responseData) : responseData
                };

                // Asenkron veya senkron olarak logla
                if (_logSettings.EnableAsyncLogging)
                {
                    _ = Task.Run(async () => 
                    {
                        await _logRepository.SaveLogAsync(logEntry);
                        
                        // Graylog'a da gönder (eğer etkinse)
                        if (_logSettings.EnableGraylog)
                        {
                            await _graylogService.SendLogAsync(logEntry);
                        }
                    });
                }
                else
                {
                    await _logRepository.SaveLogAsync(logEntry);
                    
                    // Graylog'a da gönder (eğer etkinse)
                    if (_logSettings.EnableGraylog)
                    {
                        await _graylogService.SendLogAsync(logEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"HTTP log oluşturulurken hata: {ex.Message}");
            }
        }

        #region Private Methods

        /// <summary>
        /// Yeni bir log girdisi oluşturur ve kaydeder
        /// </summary>
        private async Task CreateLogAsync(string level, string message, string source, object data, Exception exception, string userId, string userName, string userEmail)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                // Eğer user bilgileri verilmediyse ve HttpContext varsa, buradan almaya çalış
                if (httpContext != null)
                {
                    if (string.IsNullOrEmpty(userId) && httpContext.User.Identity.IsAuthenticated)
                    {
                        userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    }

                    if (string.IsNullOrEmpty(userName) && httpContext.User.Identity.IsAuthenticated)
                    {
                        userName = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    }

                    if (string.IsNullOrEmpty(userEmail) && httpContext.User.Identity.IsAuthenticated)
                    {
                        userEmail = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    }
                }

                var processedData = ProcessDataForLogging(data, exception, _logSettings.MaskSensitiveData);

                var logEntry = new LogEntry
                {
                    Level = level,
                    Message = message,
                    Source = source,
                    TraceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier,
                    UserId = userId,
                    UserName = userName,
                    UserEmail = userEmail,
                    IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                    ApplicationName = _logSettings.ApplicationName,
                    Environment = _logSettings.Environment,
                    Data = processedData,
                    HttpPath = httpContext?.Request?.Path,
                    HttpMethod = httpContext?.Request?.Method
                };

                // Asenkron logging aktifse, Task.Run ile arka plana at
                if (_logSettings.EnableAsyncLogging)
                {
                    _ = Task.Run(async () => 
                    {
                        await _logRepository.SaveLogAsync(logEntry);
                        
                        // Graylog'a da gönder (eğer etkinse)
                        if (_logSettings.EnableGraylog)
                        {
                            await _graylogService.SendLogAsync(logEntry);
                        }
                    });
                }
                else
                {
                    await _logRepository.SaveLogAsync(logEntry);
                    
                    // Graylog'a da gönder (eğer etkinse)
                    if (_logSettings.EnableGraylog)
                    {
                        await _graylogService.SendLogAsync(logEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Log oluşturulurken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Veriyi MongoDB serileştirmesi için uygun formata dönüştürür
        /// </summary>
        private object ProcessDataForLogging(object data, Exception exception = null, bool maskSensitiveData = true)
        {
            // Exception durumunda
            if (exception != null)
            {
                var exceptionData = new Dictionary<string, string>
                {
                    ["ExceptionType"] = exception.GetType().Name,
                    ["ExceptionMessage"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace,
                    ["InnerException"] = exception.InnerException?.Message
                };
                
                // Dictionary'i JObject'e dönüştür (MongoDB için daha güvenli)
                return JObject.FromObject(exceptionData);
            }
            
            // Data null ise null döndür
            if (data == null)
                return null;
                
            try
            {
                // Veriyi önce JSON String olarak serileştir
                var json = JsonConvert.SerializeObject(data);
                
                // Hassas verileri maskele (gerekirse)
                if (maskSensitiveData)
                {
                    json = MaskSensitiveDataInJson(json);
                }
                
                // JSON string'i JObject/JArray olarak parse et (MongoDB için daha güvenli)
                if (json.TrimStart().StartsWith("["))
                {
                    return JArray.Parse(json);
                }
                else
                {
                    return JObject.Parse(json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Veri serileştirme hatası: {ex.Message}. Veri string olarak kaydedilecek.");
                // Serileştirme hatası olursa, basit bir string açıklaması döndür
                return $"Serileştirilemedi: {data.GetType().Name}";
            }
        }

        /// <summary>
        /// JSON içindeki hassas verileri maskeler
        /// </summary>
        private string MaskSensitiveDataInJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return json;
                
            try
            {
                // Hassas alanları maskele
                return json.Replace("\"password\":", "\"password\":\"***\"")
                           .Replace("\"Password\":", "\"Password\":\"***\"")
                           .Replace("\"passwordHash\":", "\"passwordHash\":\"***\"")
                           .Replace("\"PasswordHash\":", "\"PasswordHash\":\"***\"")
                           .Replace("\"token\":", "\"token\":\"***\"")
                           .Replace("\"Token\":", "\"Token\":\"***\"")
                           .Replace("\"refreshToken\":", "\"refreshToken\":\"***\"")
                           .Replace("\"RefreshToken\":", "\"RefreshToken\":\"***\"")
                           .Replace("\"creditCard\":", "\"creditCard\":\"***\"")
                           .Replace("\"CreditCard\":", "\"CreditCard\":\"***\"")
                           .Replace("\"ssn\":", "\"ssn\":\"***\"")
                           .Replace("\"SSN\":", "\"SSN\":\"***\"");
            }
            catch
            {
                return json;
            }
        }

        /// <summary>
        /// Hassas verileri maskeler (artık kullanılmıyor - yerine ProcessDataForLogging kullanılıyor)
        /// </summary>
        private object MaskSensitiveData(object data)
        {
            if (data == null)
                return null;

            try
            {
                // JSON'a çevir
                var json = JsonConvert.SerializeObject(data);
                
                // Hassas alanları maskele
                json = MaskSensitiveDataInJson(json);

                return JsonConvert.DeserializeObject(json);
            }
            catch
            {
                // Maskeleme sırasında hata olursa, orijinal veriyi döndür
                return data;
            }
        }

        #endregion
    }
} 