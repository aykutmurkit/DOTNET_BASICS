using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogLib.Core.Utilities;
using LogLib.Models;
using LogLib.Repositories;
using LogLib.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LogLib.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<LogApiResponse<IEnumerable<LogEntry>>>> GetLogs(
            [FromQuery] string? level = null,
            [FromQuery] string? application = null,
            [FromQuery] string? userId = null,
            [FromQuery] string? username = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 100)
        {
            try
            {
                var logs = await _logService.GetLogsAsync(
                    level,
                    application,
                    userId,
                    username,
                    startDate,
                    endDate,
                    searchTerm,
                    skip,
                    take);

                return Ok(LogApiResponse<IEnumerable<LogEntry>>.Success(logs, "Logs retrieved successfully"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("Error retrieving logs", ex);
                return StatusCode(500, LogApiResponse<IEnumerable<LogEntry>>.ServerError(ex.Message));
            }
        }

        [HttpPost("debug")]
        public async Task<ActionResult<LogApiResponse<object>>> LogDebug([FromBody] LogRequest request)
        {
            try
            {
                await _logService.LogDebugAsync(
                    request.Message,
                    request.UserId,
                    request.Username,
                    request.UserEmail,
                    request.CorrelationId,
                    request.AdditionalData);

                return Ok(LogApiResponse<object>.Success(null, "Debug log created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, LogApiResponse<object>.ServerError(ex.Message));
            }
        }

        [HttpPost("info")]
        public async Task<ActionResult<LogApiResponse<object>>> LogInfo([FromBody] LogRequest request)
        {
            try
            {
                await _logService.LogInfoAsync(
                    request.Message,
                    request.UserId,
                    request.Username,
                    request.UserEmail,
                    request.CorrelationId,
                    request.AdditionalData);

                return Ok(LogApiResponse<object>.Success(null, "Info log created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, LogApiResponse<object>.ServerError(ex.Message));
            }
        }

        [HttpPost("warning")]
        public async Task<ActionResult<LogApiResponse<object>>> LogWarning([FromBody] LogRequest request)
        {
            try
            {
                await _logService.LogWarningAsync(
                    request.Message,
                    request.UserId,
                    request.Username,
                    request.UserEmail,
                    request.CorrelationId,
                    request.AdditionalData);

                return Ok(LogApiResponse<object>.Success(null, "Warning log created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, LogApiResponse<object>.ServerError(ex.Message));
            }
        }

        [HttpPost("error")]
        public async Task<ActionResult<LogApiResponse<object>>> LogError([FromBody] ErrorLogRequest request)
        {
            try
            {
                Exception? exception = null;
                if (!string.IsNullOrEmpty(request.ExceptionMessage))
                {
                    exception = new Exception(request.ExceptionMessage);
                }

                await _logService.LogErrorAsync(
                    request.Message,
                    exception,
                    request.UserId,
                    request.Username,
                    request.UserEmail,
                    request.CorrelationId,
                    request.AdditionalData);

                return Ok(LogApiResponse<object>.Success(null, "Error log created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, LogApiResponse<object>.ServerError(ex.Message));
            }
        }

        [HttpPost("critical")]
        public async Task<ActionResult<LogApiResponse<object>>> LogCritical([FromBody] ErrorLogRequest request)
        {
            try
            {
                Exception? exception = null;
                if (!string.IsNullOrEmpty(request.ExceptionMessage))
                {
                    exception = new Exception(request.ExceptionMessage);
                }

                await _logService.LogCriticalAsync(
                    request.Message,
                    exception,
                    request.UserId,
                    request.Username,
                    request.UserEmail,
                    request.CorrelationId,
                    request.AdditionalData);

                return Ok(LogApiResponse<object>.Success(null, "Critical log created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, LogApiResponse<object>.ServerError(ex.Message));
            }
        }

        [HttpPost("security")]
        public async Task<ActionResult<LogApiResponse<object>>> LogSecurity([FromBody] SecurityLogRequest request)
        {
            try
            {
                await _logService.LogSecurityAsync(
                    request.Message,
                    request.UserId,
                    request.Username,
                    request.UserEmail,
                    request.IpAddress,
                    request.CorrelationId,
                    request.AdditionalData);

                return Ok(LogApiResponse<object>.Success(null, "Security log created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, LogApiResponse<object>.ServerError(ex.Message));
            }
        }

        [HttpPost("clear")]
        public async Task<ActionResult<LogApiResponse<object>>> ClearLogs()
        {
            try
            {
                var repo = HttpContext.RequestServices.GetService<ILogRepository>();
                if (repo != null)
                {
                    await repo.ClearAllLogsAsync();
                    return Ok(LogApiResponse<object>.Success(null, "Logs cleared successfully"));
                }
                return BadRequest(LogApiResponse<object>.Error("Log repository not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, LogApiResponse<object>.ServerError(ex.Message));
            }
        }
    }

    public class LogRequest
    {
        public required string Message { get; set; }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? UserEmail { get; set; }
        public string? CorrelationId { get; set; }
        public Dictionary<string, object>? AdditionalData { get; set; }
    }

    public class ErrorLogRequest : LogRequest
    {
        public string? ExceptionMessage { get; set; }
    }

    public class SecurityLogRequest : LogRequest
    {
        public string? IpAddress { get; set; }
    }
} 