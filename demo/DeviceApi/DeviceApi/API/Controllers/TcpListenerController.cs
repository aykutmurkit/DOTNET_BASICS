using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceApi.TCPListener.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.Utilities;
using LogLibrary.Core.Interfaces;
using DeviceApi.TCPListener.Models.Devices;
using System.Linq;
using System;

namespace DeviceApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TcpListenerController : ControllerBase
    {
        private readonly ILogger<TcpListenerController> _logger;
        private readonly ITcpListenerService _tcpListenerService;
        private readonly IDeviceVerificationService _deviceVerificationService;
        private readonly ILogService _logService;

        public TcpListenerController(
            ILogger<TcpListenerController> logger,
            ITcpListenerService tcpListenerService,
            IDeviceVerificationService deviceVerificationService,
            ILogService logService)
        {
            _logger = logger;
            _tcpListenerService = tcpListenerService;
            _deviceVerificationService = deviceVerificationService;
            _logService = logService;
        }

        /// <summary>
        /// TCP Listener servisinin durumunu ve istatistiklerini getirir
        /// </summary>
        /// <returns>TCP Listener'ın durum bilgisini içeren yanıt</returns>
        [HttpGet("status")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetStatus()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            await _logService.LogInfoAsync(
                "TCP Listener durum bilgisi ve istatistikler alınıyor", 
                "TcpListenerController.GetStatus", 
                new { UserId = userId, UserName = userName });
            
            var stats = _tcpListenerService.GetStatistics();
            var approvedDevicesCount = _deviceVerificationService.GetApprovedDevices().Count();
            var unapprovedDevicesCount = _deviceVerificationService.GetUnapprovedDevices().Count();
            
            var result = new 
            {
                // Temel bilgiler
                IsRunning = stats.IsRunning,
                IpAddress = stats.IpAddress,
                Port = stats.Port,
                
                // Bağlantı bilgileri
                ActiveConnections = stats.ActiveConnections,
                MaximumConnections = stats.MaximumConnections,
                TotalConnectionsReceived = stats.TotalConnectionsReceived,
                ConnectionsLastMinute = stats.ConnectionsLastMinute,
                
                // Cihaz ve thread bilgileri
                ApprovedDevices = approvedDevicesCount,
                UnapprovedDevices = unapprovedDevicesCount,
                TotalDevices = approvedDevicesCount + unapprovedDevicesCount,
                ActiveThreads = stats.ActiveThreads,
                
                // Zaman bilgileri
                StartTime = stats.StartTime,
                Uptime = stats.Uptime,
                ServerTime = DateTime.Now,
                
                // Aktif bağlantı bilgileri
                ActiveClientAddresses = stats.ActiveClientAddresses.Take(20).ToList(), // Sadece ilk 20 adresi göster
                TotalActiveClients = stats.ActiveClientAddresses.Count,
                
                // Hız sınırlama ve kara liste istatistikleri
                RateLimit = stats.RateLimit != null ? new
                {
                    BlacklistedImeiCount = stats.RateLimit.BlacklistedImeiCount,
                    RateLimitedImeiCount = stats.RateLimit.RateLimitedImeiCount,
                    BlacklistDurationSeconds = stats.RateLimit.BlacklistDurationSeconds,
                    RateLimitConfig = stats.RateLimit.RateLimitConfig
                } : null,
                
                // Mesaj işleme istatistikleri
                MessageStats = stats.MessageStats != null ? new
                {
                    TotalProcessedMessages = stats.MessageStats.TotalProcessedMessages,
                    ThrottledLogCount = stats.MessageStats.ThrottledLogCount,
                    LogEfficiencyPercent = stats.MessageStats.TotalProcessedMessages > 0 
                        ? Math.Round(100.0 * stats.MessageStats.ThrottledLogCount / stats.MessageStats.TotalProcessedMessages, 2) 
                        : 0,
                    LastSuccessfulHandshake = stats.MessageStats.LastSuccessfulHandshake,
                    LastRejectedHandshake = stats.MessageStats.LastRejectedHandshake,
                    TimeSinceLastSuccess = stats.MessageStats.LastSuccessfulHandshake.HasValue 
                        ? (DateTime.Now - stats.MessageStats.LastSuccessfulHandshake.Value).TotalMinutes.ToString("F1") + " dakika"
                        : "Yok",
                    TimeSinceLastReject = stats.MessageStats.LastRejectedHandshake.HasValue 
                        ? (DateTime.Now - stats.MessageStats.LastRejectedHandshake.Value).TotalMinutes.ToString("F1") + " dakika"
                        : "Yok"
                } : null
            };
            
            return Ok(ApiResponse<object>.Success(result, "TCP Listener durum bilgisi ve istatistikler"));
        }
        
        /// <summary>
        /// Onaylı cihazların listesini getirir
        /// </summary>
        /// <returns>Onaylı cihazların detaylı bilgilerini içeren liste</returns>
        [HttpGet("approved-devices")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ApprovedDeviceDto>>), 200)]
        public async Task<IActionResult> GetApprovedDevices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            await _logService.LogInfoAsync(
                "Onaylı cihazlar detaylı listesi alınıyor", 
                "TcpListenerController.GetApprovedDevices", 
                new { UserId = userId, UserName = userName });
            
            var devices = _deviceVerificationService.GetApprovedDevicesWithDetails();
            return Ok(ApiResponse<IEnumerable<ApprovedDeviceDto>>.Success(devices, "Onaylı cihazlar detaylı listesi"));
        }
        
        /// <summary>
        /// Onaysız cihazların listesini getirir
        /// </summary>
        /// <returns>Onaysız cihazların bilgilerini içeren liste</returns>
        [HttpGet("unapproved-devices")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnapprovedDeviceDto>>), 200)]
        public async Task<IActionResult> GetUnapprovedDevices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            await _logService.LogInfoAsync(
                "Onaysız cihazlar detaylı listesi alınıyor", 
                "TcpListenerController.GetUnapprovedDevices", 
                new { UserId = userId, UserName = userName });
            
            var devices = _deviceVerificationService.GetUnapprovedDevicesWithDetails();
            return Ok(ApiResponse<IEnumerable<UnapprovedDeviceDto>>.Success(devices, "Onaysız cihazlar detaylı listesi"));
        }
        
        /// <summary>
        /// TCP Listener servisini başlatır
        /// </summary>
        /// <returns>İşlem sonucunu içeren yanıt</returns>
        [HttpPost("start")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> StartListener()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            await _logService.LogInfoAsync(
                "TCP Listener servisi başlatılıyor", 
                "TcpListenerController.StartListener", 
                new { UserId = userId, UserName = userName });
            
            if (_tcpListenerService.IsRunning())
            {
                _logger.LogWarning("TCP Listener servisi zaten çalışıyor");
                
                await _logService.LogWarningAsync(
                    "TCP Listener servisi zaten çalışıyor", 
                    "TcpListenerController.StartListener", 
                    new { UserId = userId, UserName = userName });
                
                return BadRequest(ApiResponse<object>.Error("TCP Listener servisi zaten çalışıyor"));
            }
            
            try
            {
                await _tcpListenerService.StartAsync();
                
                await _logService.LogInfoAsync(
                    "TCP Listener servisi başarıyla başlatıldı", 
                    "TcpListenerController.StartListener", 
                    new { UserId = userId, UserName = userName });
                
                return Ok(ApiResponse<object>.Success(new { IsRunning = true }, "TCP Listener servisi başlatıldı"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "TCP Listener servisi başlatılırken hata oluştu", 
                    "TcpListenerController.StartListener", 
                    ex, userId, userName);
                
                return BadRequest(ApiResponse<object>.Error("TCP Listener servisi başlatılırken hata oluştu: " + ex.Message));
            }
        }
        
        /// <summary>
        /// TCP Listener servisini durdurur
        /// </summary>
        /// <returns>İşlem sonucunu içeren yanıt</returns>
        [HttpPost("stop")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> StopListener()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            await _logService.LogInfoAsync(
                "TCP Listener servisi durduruluyor", 
                "TcpListenerController.StopListener", 
                new { UserId = userId, UserName = userName });
            
            if (!_tcpListenerService.IsRunning())
            {
                _logger.LogWarning("TCP Listener servisi zaten durdurulmuş");
                
                await _logService.LogWarningAsync(
                    "TCP Listener servisi zaten durdurulmuş", 
                    "TcpListenerController.StopListener", 
                    new { UserId = userId, UserName = userName });
                
                return BadRequest(ApiResponse<object>.Error("TCP Listener servisi zaten durdurulmuş"));
            }
            
            try
            {
                await _tcpListenerService.StopAsync();
                
                await _logService.LogInfoAsync(
                    "TCP Listener servisi başarıyla durduruldu", 
                    "TcpListenerController.StopListener", 
                    new { UserId = userId, UserName = userName });
                
                return Ok(ApiResponse<object>.Success(new { IsRunning = false }, "TCP Listener servisi durduruldu"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "TCP Listener servisi durdurulurken hata oluştu", 
                    "TcpListenerController.StopListener", 
                    ex, userId, userName);
                
                return BadRequest(ApiResponse<object>.Error("TCP Listener servisi durdurulurken hata oluştu: " + ex.Message));
            }
        }
        
        /// <summary>
        /// TCP Listener servisini yeniden başlatır
        /// </summary>
        /// <returns>İşlem sonucunu içeren yanıt</returns>
        [HttpPost("restart")]
        [Authorize(Roles = "Admin,Developer")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> RestartListener()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            await _logService.LogInfoAsync(
                "TCP Listener servisi yeniden başlatılıyor", 
                "TcpListenerController.RestartListener", 
                new { UserId = userId, UserName = userName });
            
            try
            {
                if (_tcpListenerService.IsRunning())
                {
                    await _tcpListenerService.StopAsync();
                }
                
                await _tcpListenerService.StartAsync();
                
                await _logService.LogInfoAsync(
                    "TCP Listener servisi başarıyla yeniden başlatıldı", 
                    "TcpListenerController.RestartListener", 
                    new { UserId = userId, UserName = userName });
                
                return Ok(ApiResponse<object>.Success(new { IsRunning = true }, "TCP Listener servisi yeniden başlatıldı"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    "TCP Listener servisi yeniden başlatılırken hata oluştu", 
                    "TcpListenerController.RestartListener", 
                    ex, userId, userName);
                
                return BadRequest(ApiResponse<object>.Error("TCP Listener servisi yeniden başlatılırken hata oluştu: " + ex.Message));
            }
        }
    }
} 