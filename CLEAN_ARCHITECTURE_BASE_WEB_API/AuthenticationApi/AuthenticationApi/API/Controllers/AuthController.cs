using AuthenticationApi.Business.Services.Interfaces;
using AuthenticationApi.Core.Logging;
using Core.Utilities;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogService _logService;

        public AuthController(IAuthService authService, ILogService logService)
        {
            _authService = authService;
            _logService = logService;
        }

        /// <summary>
        /// Kullanıcı giriş işlemi
        /// </summary>
        [HttpPost("login")]
        [EnableRateLimiting("api_auth_login")]
        public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Auth", "Login", request);
                
                var result = await _authService.LoginAsync(request);
                
                // 2FA gerekiyorsa farklı bir yanıt döndür
                if (result is TwoFactorRequiredResponse twoFactorResponse)
                {
                    var response = ApiResponse<TwoFactorRequiredResponse>.Success(twoFactorResponse, "İki faktörlü kimlik doğrulama gerekli", 200);
                    
                    // Yanıt logla
                    _logService.LogResponse(HttpContext, "Auth", "Login", response, 200);
                    
                    return Ok(response);
                }
                
                // Normal giriş yanıtı
                var successResponse = ApiResponse<AuthResponse>.Success((AuthResponse)result, "Giriş başarılı");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Auth", "Login", successResponse, 200);
                
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Auth", "Login", ex);
                
                if (ex.Message.Contains("Kullanıcı adı veya şifre"))
                {
                    return StatusCode(401, ApiResponse<AuthResponse>.Unauthorized(ex.Message));
                }
                return BadRequest(ApiResponse<AuthResponse>.Error(ex.Message));
            }
        }

        /// <summary>
        /// İki faktörlü kimlik doğrulama
        /// </summary>
        [HttpPost("verify-2fa")]
        [EnableRateLimiting("api_auth_verify-2fa")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> VerifyTwoFactor([FromBody] TwoFactorVerifyRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Auth", "VerifyTwoFactor", request);
                
                var result = await _authService.VerifyTwoFactorAsync(request);
                var response = ApiResponse<AuthResponse>.Success(result, "İki faktörlü kimlik doğrulama başarılı");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Auth", "VerifyTwoFactor", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Auth", "VerifyTwoFactor", ex);
                
                if (ex.Message.Contains("Doğrulama kodu geçersiz"))
                {
                    return BadRequest(ApiResponse<AuthResponse>.Error(ex.Message));
                }
                return StatusCode(500, ApiResponse<AuthResponse>.ServerError(ex.Message));
            }
        }

        /// <summary>
        /// İki faktörlü kimlik doğrulama ayarları
        /// </summary>
        [Authorize]
        [HttpGet("2fa-status")]
        public async Task<ActionResult<ApiResponse<TwoFactorStatusResponse>>> GetTwoFactorStatus()
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Auth", "GetTwoFactorStatus", null);
                
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _authService.GetTwoFactorStatusAsync(userId);
                var response = ApiResponse<TwoFactorStatusResponse>.Success(result, "2FA durumu");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Auth", "GetTwoFactorStatus", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Auth", "GetTwoFactorStatus", ex);
                
                return StatusCode(500, ApiResponse<TwoFactorStatusResponse>.ServerError(ex.Message));
            }
        }

        /// <summary>
        /// İki faktörlü kimlik doğrulama ayarlarını güncelleme
        /// </summary>
        [Authorize]
        [HttpPost("setup-2fa")]
        public async Task<ActionResult<ApiResponse<TwoFactorStatusResponse>>> SetupTwoFactor([FromBody] TwoFactorSetupRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Auth", "SetupTwoFactor", request);
                
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _authService.SetupTwoFactorAsync(userId, request);
                var response = ApiResponse<TwoFactorStatusResponse>.Success(result, "2FA ayarları güncellendi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Auth", "SetupTwoFactor", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Auth", "SetupTwoFactor", ex);
                
                if (ex.Message.Contains("Mevcut şifre hatalı"))
                {
                    return BadRequest(ApiResponse<TwoFactorStatusResponse>.Error(ex.Message));
                }
                if (ex.Message.Contains("sistem tarafından zorunlu"))
                {
                    return StatusCode(403, ApiResponse<TwoFactorStatusResponse>.Forbidden(ex.Message));
                }
                return StatusCode(500, ApiResponse<TwoFactorStatusResponse>.ServerError(ex.Message));
            }
        }

        /// <summary>
        /// Kullanıcı kayıt işlemi
        /// </summary>
        [HttpPost("register")]
        [EnableRateLimiting("api_auth_register")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Auth", "Register", request);
                
                var result = await _authService.RegisterAsync(request);
                var response = ApiResponse<AuthResponse>.Created(result, "Kullanıcı başarıyla kaydedildi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Auth", "Register", response, 201);
                
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Auth", "Register", ex);
                
                if (ex.Message.Contains("zaten kullanılıyor"))
                {
                    return StatusCode(409, ApiResponse<AuthResponse>.Conflict(ex.Message));
                }
                return BadRequest(ApiResponse<AuthResponse>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Token yenileme işlemi
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Auth", "RefreshToken", request);
                
                var result = await _authService.RefreshTokenAsync(request);
                var response = ApiResponse<AuthResponse>.Success(result, "Token başarıyla yenilendi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Auth", "RefreshToken", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Auth", "RefreshToken", ex);
                
                if (ex.Message.Contains("Geçersiz") || ex.Message.Contains("süresi dolmuş"))
                {
                    return StatusCode(401, ApiResponse<AuthResponse>.Unauthorized(ex.Message));
                }
                return BadRequest(ApiResponse<AuthResponse>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Şifre değiştirme işlemi
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Auth", "ChangePassword", request);
                
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _authService.ChangePasswordAsync(userId, request);
                var response = ApiResponse<object>.Success(null, "Şifre başarıyla değiştirildi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Auth", "ChangePassword", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Auth", "ChangePassword", ex);
                
                if (ex.Message.Contains("Mevcut şifre hatalı"))
                {
                    return BadRequest(ApiResponse<object>.Error(ex.Message));
                }
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                return StatusCode(500, ApiResponse<object>.ServerError(ex.Message));
            }
        }

        /// <summary>
        /// Şifre sıfırlama talebi
        /// </summary>
        [HttpPost("forgot-password")]
        [EnableRateLimiting("api_auth_forgot-password")]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Auth", "ForgotPassword", request);
                
                await _authService.ForgotPasswordAsync(request);
                var response = ApiResponse<object>.Success(null, "Şifre sıfırlama talimatları e-posta adresinize gönderildi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Auth", "ForgotPassword", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Auth", "ForgotPassword", ex);
                
                return StatusCode(500, ApiResponse<object>.ServerError(ex.Message));
            }
        }

        /// <summary>
        /// Şifre sıfırlama işlemi
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Auth", "ResetPassword", request);
                
                await _authService.ResetPasswordAsync(request);
                var response = ApiResponse<object>.Success(null, "Şifreniz başarıyla sıfırlandı");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Auth", "ResetPassword", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Auth", "ResetPassword", ex);
                
                if (ex.Message.Contains("geçersiz") || ex.Message.Contains("süresi dolmuş"))
                {
                    return BadRequest(ApiResponse<object>.Error(ex.Message));
                }
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                return StatusCode(500, ApiResponse<object>.ServerError(ex.Message));
            }
        }
    }
} 