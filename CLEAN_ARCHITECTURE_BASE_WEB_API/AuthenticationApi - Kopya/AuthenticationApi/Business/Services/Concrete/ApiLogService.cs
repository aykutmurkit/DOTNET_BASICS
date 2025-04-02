using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationApi.Business.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AuthenticationApi.Business.Services.Concrete
{
    public class ApiLogService : IApiLogService
    {
        private readonly ILogger<ApiLogService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiLogService(ILogger<ApiLogService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task LogInfoAsync(string message, HttpContext context = null)
        {
            LogMessage(LogLevel.Information, message, null, context);
            return Task.CompletedTask;
        }

        public Task LogWarningAsync(string message, HttpContext context = null)
        {
            LogMessage(LogLevel.Warning, message, null, context);
            return Task.CompletedTask;
        }

        public Task LogErrorAsync(string message, Exception exception = null, HttpContext context = null)
        {
            LogMessage(LogLevel.Error, message, exception, context);
            return Task.CompletedTask;
        }

        private void LogMessage(LogLevel logLevel, string message, Exception exception = null, HttpContext context = null)
        {
            context ??= _httpContextAccessor.HttpContext;

            var userId = context?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = context?.User.FindFirstValue(ClaimTypes.Name);
            var path = context?.Request.Path;
            var traceId = Activity.Current?.Id ?? context?.TraceIdentifier;

            if (exception != null)
            {
                _logger.Log(logLevel, exception, 
                    "TraceId: {TraceId}, UserId: {UserId}, Username: {Username}, Path: {Path}, Message: {Message}", 
                    traceId, userId, username, path, message);
            }
            else
            {
                _logger.Log(logLevel, 
                    "TraceId: {TraceId}, UserId: {UserId}, Username: {Username}, Path: {Path}, Message: {Message}", 
                    traceId, userId, username, path, message);
            }
        }
    }
}