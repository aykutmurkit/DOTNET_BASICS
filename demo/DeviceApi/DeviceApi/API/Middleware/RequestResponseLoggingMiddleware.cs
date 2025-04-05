using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DeviceApi.Business.Services.Interfaces;
using DeviceApi.Models.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeviceApi.API.Middleware
{
    /// <summary>
    /// HTTP istek ve yanıtlarını loglayan middleware
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly string[] _excludedPaths;
        
        public RequestResponseLoggingMiddleware(
            RequestDelegate next, 
            ILogger<RequestResponseLoggingMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _excludedPaths = configuration.GetSection("LogSettings:ExcludedPaths").Get<string[]>() ?? Array.Empty<string>();
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            // Belirli endpoint'ler loglama dışında bırakılabilir
            if (ShouldSkipLogging(context.Request.Path))
            {
                await _next(context);
                return;
            }
            
            var log = new RequestResponseLog
            {
                Path = context.Request.Path,
                HttpMethod = context.Request.Method,
                QueryString = context.Request.QueryString.ToString(),
                TraceId = context.TraceIdentifier,
                UserIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                Timestamp = DateTime.UtcNow
            };

            // Kimliği doğrulanmış kullanıcı varsa, kullanıcı bilgilerini ekle
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "nameid");
                var usernameClaim = context.User.Claims.FirstOrDefault(c => c.Type == "unique_name");

                log.UserId = userIdClaim?.Value;
                log.Username = usernameClaim?.Value;
            }

            // İsteği kaydet
            var originalRequestBody = context.Request.Body;
            await using var requestBodyStream = _recyclableMemoryStreamManager.GetStream();
            context.Request.Body = requestBodyStream;

            if (context.Request.ContentLength > 0)
            {
                await originalRequestBody.CopyToAsync(requestBodyStream);
                requestBodyStream.Position = 0;

                var requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
                requestBodyStream.Position = 0;

                // Hassas verileri maskeleme
                log.RequestBody = MaskSensitiveData(requestBodyText);
                log.RequestSize = context.Request.ContentLength ?? 0;
            }

            // Yanıtı yakalamak için orijinal yanıt akışını değiştir
            var originalResponseBody = context.Response.Body;
            await using var responseBodyStream = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBodyStream;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            finally
            {
                watch.Stop();
                log.ExecutionTime = watch.ElapsedMilliseconds;
                log.StatusCode = context.Response.StatusCode;

                // Yanıt gövdesini oku
                responseBodyStream.Position = 0;
                var responseBodyText = await new StreamReader(responseBodyStream, Encoding.UTF8).ReadToEndAsync();
                responseBodyStream.Position = 0;

                // Hassas verileri maskeleme
                log.ResponseBody = MaskSensitiveData(responseBodyText);
                log.ResponseSize = responseBodyStream.Length;

                // Yanıtı kullanıcıya geri gönder
                await responseBodyStream.CopyToAsync(originalResponseBody);

                // Logu kaydet
                try
                {
                    var logRepository = context.RequestServices.GetRequiredService<ILogRepository>();
                    await logRepository.SaveRequestResponseLogAsync(log);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "HTTP request/response log kaydedilemedi");
                }
            }

            // İstek gövdesini orijinal haline döndür
            context.Request.Body = originalRequestBody;
        }

        private bool ShouldSkipLogging(PathString path)
        {
            return _excludedPaths.Any(excludedPath => path.StartsWithSegments(excludedPath));
        }

        private string MaskSensitiveData(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            // Yaygın hassas alanları maskele
            var sensitiveFieldPatterns = new[]
            {
                "\"password\"\\s*:\\s*\"[^\"]*\"",
                "\"confirmPassword\"\\s*:\\s*\"[^\"]*\"",
                "\"currentPassword\"\\s*:\\s*\"[^\"]*\"",
                "\"newPassword\"\\s*:\\s*\"[^\"]*\"",
                "\"token\"\\s*:\\s*\"[^\"]*\"",
                "\"refreshToken\"\\s*:\\s*\"[^\"]*\""
            };

            var maskedContent = content;
            foreach (var pattern in sensitiveFieldPatterns)
            {
                maskedContent = System.Text.RegularExpressions.Regex.Replace(
                    maskedContent,
                    pattern,
                    m => m.Value.Split(new[] { ':' }, 2)[0] + ": \"***MASKED***\"",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
            }

            return maskedContent;
        }
    }
} 