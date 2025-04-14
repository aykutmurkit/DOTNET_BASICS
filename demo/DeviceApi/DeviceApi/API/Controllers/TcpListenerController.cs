using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceApi.TCPListener.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.Utilities;
using LogLibrary.Core.Interfaces;

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
        /// TCP Listener servisinin durumunu getirir
        /// </summary>
        /// <returns>TCP Listener'ın durum bilgisini içeren yanıt</returns>
        [HttpGet("status")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetStatus()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            await _logService.LogInfoAsync(
                "TCP Listener durum bilgisi alınıyor", 
                "TcpListenerController.GetStatus", 
                new { UserId = userId, UserName = userName });
            
            var isRunning = _tcpListenerService.IsRunning();
            var connectedClients = _tcpListenerService.GetConnectedClientsCount();
            
            var result = new 
            {
                IsRunning = isRunning,
                ConnectedClients = connectedClients,
                Port = _tcpListenerService.Port,
                IpAddress = _tcpListenerService.IpAddress
            };
            
            return Ok(ApiResponse<object>.Success(result, "TCP Listener durum bilgisi"));
        }
        
        /// <summary>
        /// Onaylı cihazların listesini getirir
        /// </summary>
        /// <returns>Onaylı cihazların IMEI numaralarını içeren liste</returns>
        [HttpGet("approved-devices")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), 200)]
        public async Task<IActionResult> GetApprovedDevices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            await _logService.LogInfoAsync(
                "Onaylı cihazlar listesi alınıyor", 
                "TcpListenerController.GetApprovedDevices", 
                new { UserId = userId, UserName = userName });
            
            var devices = _deviceVerificationService.GetApprovedDevices();
            return Ok(ApiResponse<IEnumerable<string>>.Success(devices, "Onaylı cihazlar listesi"));
        }
        
        /// <summary>
        /// Onaysız cihazların listesini getirir
        /// </summary>
        /// <returns>Onaysız cihazların IMEI numaralarını içeren liste</returns>
        [HttpGet("unapproved-devices")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), 200)]
        public async Task<IActionResult> GetUnapprovedDevices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            
            await _logService.LogInfoAsync(
                "Onaysız cihazlar listesi alınıyor", 
                "TcpListenerController.GetUnapprovedDevices", 
                new { UserId = userId, UserName = userName });
            
            var devices = _deviceVerificationService.GetUnapprovedDevices();
            return Ok(ApiResponse<IEnumerable<string>>.Success(devices, "Onaysız cihazlar listesi"));
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