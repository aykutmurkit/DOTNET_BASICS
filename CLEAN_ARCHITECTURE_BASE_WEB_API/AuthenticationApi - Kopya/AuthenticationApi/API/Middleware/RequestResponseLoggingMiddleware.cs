using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Business.Services.Interfaces;
using AuthenticationApi.Models.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AuthenticationApi.API.Middleware
{
    /// <summary>
    /// HTTP istek ve yanıtlarını loglayan middleware
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _streamManager;
        private readonly string[] _excludedPaths;
        
        public RequestResponseLoggingMiddleware(
            RequestDelegate next, 
            ILogger<RequestResponseLoggingMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _streamManager = new RecyclableMemoryStreamManager();
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
            
            // İstek logu
            await LogRequest(context);
            
            // Yanıt body'sini yakalamak için orijinal response stream'i değiştir
            var originalBodyStream = context.Response.Body;
            
            using var responseBody = _streamManager.GetStream();
            context.Response.Body = responseBody;
            
            try
            {
                // Sonraki middleware'i çağır
                await _next(context);
                
                // Yanıt logu oluştur
                await LogResponse(context, responseBody);
                
                // Yanıt verilerini orijinal stream'e kopyala
                responseBody.Position = 0;
                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                // Response body'i orijinal hale getir
                context.Response.Body = originalBodyStream;
            }
        }

        private bool ShouldSkipLogging(PathString path)
        {
            return _excludedPaths.Any(excludedPath => path.StartsWithSegments(excludedPath));
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            
            var requestTime = DateTime.UtcNow;
            var requestBody = await ReadRequestBody(context.Request);
            
            // Log request details
            _logger.LogInformation(
                "HTTP Request - {Method} {Scheme}://{Host}{Path}{QueryString} - Body: {RequestBody}",
                context.Request.Method,
                context.Request.Scheme,
                context.Request.Host,
                context.Request.Path,
                context.Request.QueryString,
                requestBody
            );
            
            // Reset the request body position for other middleware to read it
            context.Request.Body.Position = 0;
        }
        
        private async Task LogResponse(HttpContext context, MemoryStream responseStream)
        {
            responseStream.Position = 0;
            var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
            
            // Log response details
            _logger.LogInformation(
                "HTTP Response - StatusCode: {StatusCode} - Body: {ResponseBody}",
                context.Response.StatusCode,
                responseBody
            );
            
            responseStream.Position = 0;
        }
        
        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            using var reader = new StreamReader(
                request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);
                
            var body = await reader.ReadToEndAsync();
            
            // If body contains sensitive data, consider masking it here
            return body;
        }
    }

    // Extension method for app.UseRequestResponseLogging() in Program.cs
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
} 