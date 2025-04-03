using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthApi.Business.Services.Interfaces;
using AuthApi.Models.Logs;
using Microsoft.AspNetCore.Http;

namespace AuthApi.Business.Services.Concrete
{
    public class ApiLogService : IApiLogService
    {
        private readonly ILogRepository _logRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiLogService(ILogRepository logRepository, IHttpContextAccessor httpContextAccessor)
        {
            _logRepository = logRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogInfoAsync(string message, HttpContext context = null)
        {
            await LogMessageAsync("Info", message, null, context);
        }

        public async Task LogWarningAsync(string message, HttpContext context = null)
        {
            await LogMessageAsync("Warning", message, null, context);
        }

        public async Task LogErrorAsync(string message, Exception exception = null, HttpContext context = null)
        {
            await LogMessageAsync("Error", message, exception, context);
        }

        private async Task LogMessageAsync(string level, string message, Exception exception = null, HttpContext context = null)
        {
            context ??= _httpContextAccessor.HttpContext;

            var apiLog = new ApiLog
            {
                Level = level,
                Message = message,
                Exception = exception?.ToString(),
                TraceId = Activity.Current?.Id ?? context?.TraceIdentifier,
                UserId = context?.User.FindFirstValue(ClaimTypes.NameIdentifier),
                Username = context?.User.FindFirstValue(ClaimTypes.Name),
                Path = context?.Request.Path,
                Timestamp = DateTime.UtcNow
            };

            await _logRepository.SaveApiLogAsync(apiLog);
        }
    }
}