using AuthApi.Business.Services.Interfaces;
using Core.Utilities;
using Entities.Dtos;
using LogLibrary.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace AuthApi.Controllers
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
            await _logService.LogInfoAsync(
                message: "Giriş işlemi başlatıldı", 
                source: "AuthController.Login",
                data: new { Username = request.Username });
            
            try
            {
                var result = await _authService.LoginAsync(request);
                
                // 2FA gerekiyorsa farklı bir yanıt döndür
                if (result is TwoFactorRequiredResponse twoFactorResponse)
                {
                    await _logService.LogInfoAsync(
                        message: "İki faktörlü kimlik doğrulama gerekli", 
                        source: "AuthController.Login",
                        data: new { Username = request.Username },
                        userId: twoFactorResponse.UserId.ToString(),
                        userName: request.Username);
                        
                    return Ok(ApiResponse<TwoFactorRequiredResponse>.Success(twoFactorResponse, "İki faktörlü kimlik doğrulama gerekli", 200));
                }
                
                // Normal giriş yanıtı
                var authResponse = (AuthResponse)result;
                
                await _logService.LogInfoAsync(
                    message: "Giriş başarılı", 
                    source: "AuthController.Login",
                    data: new { Username = request.Username, TokenExpiration = authResponse.AccessToken.ExpiresAt },
                    userId: authResponse.User.Id.ToString(),
                    userName: authResponse.User.Username);
                    
                return Ok(ApiResponse<AuthResponse>.Success(authResponse, "Giriş başarılı"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    message: "Giriş işlemi başarısız", 
                    source: "AuthController.Login",
                    exception: ex,
                    userName: request.Username);
                    
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
            await _logService.LogInfoAsync(
                message: "2FA doğrulama işlemi başlatıldı", 
                source: "AuthController.VerifyTwoFactor",
                data: new { UserId = request.UserId });
                
            try
            {
                var result = await _authService.VerifyTwoFactorAsync(request);
                
                await _logService.LogInfoAsync(
                    message: "2FA doğrulama başarılı", 
                    source: "AuthController.VerifyTwoFactor",
                    userId: result.User.Id.ToString(),
                    userName: result.User.Username);
                    
                return Ok(ApiResponse<AuthResponse>.Success(result, "İki faktörlü kimlik doğrulama başarılı"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    message: "2FA doğrulama başarısız", 
                    source: "AuthController.VerifyTwoFactor",
                    exception: ex,
                    userId: request.UserId.ToString());
                    
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
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _authService.GetTwoFactorStatusAsync(userId);
                return Ok(ApiResponse<TwoFactorStatusResponse>.Success(result, "2FA durumu"));
            }
            catch (Exception ex)
            {
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
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _authService.SetupTwoFactorAsync(userId, request);
                return Ok(ApiResponse<TwoFactorStatusResponse>.Success(result, "2FA ayarları güncellendi"));
            }
            catch (Exception ex)
            {
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
            await _logService.LogInfoAsync(
                message: "Kullanıcı kayıt işlemi başlatıldı", 
                source: "AuthController.Register",
                data: new { Username = request.Username, Email = request.Email });
                
            try
            {
                var result = await _authService.RegisterAsync(request);
                
                await _logService.LogInfoAsync(
                    message: "Kullanıcı başarıyla kaydedildi", 
                    source: "AuthController.Register",
                    userId: result.User.Id.ToString(),
                    userName: result.User.Username,
                    userEmail: request.Email);
                    
                var response = ApiResponse<AuthResponse>.Created(result, "Kullanıcı başarıyla kaydedildi");
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    message: "Kullanıcı kayıt işlemi başarısız", 
                    source: "AuthController.Register",
                    exception: ex,
                    userName: request.Username,
                    userEmail: request.Email);
                    
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
                var result = await _authService.RefreshTokenAsync(request);
                return Ok(ApiResponse<AuthResponse>.Success(result, "Token başarıyla yenilendi"));
            }
            catch (Exception ex)
            {
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
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _authService.ChangePasswordAsync(userId, request);
                return Ok(ApiResponse<object>.Success(null, "Şifre başarıyla değiştirildi"));
            }
            catch (Exception ex)
            {
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
                await _authService.ForgotPasswordAsync(request);
                return Ok(ApiResponse<object>.Success(null, "Şifre sıfırlama talimatları e-posta adresinize gönderildi"));
            }
            catch (Exception ex)
            {
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
                await _authService.ResetPasswordAsync(request);
                return Ok(ApiResponse<object>.Success(null, "Şifreniz başarıyla sıfırlandı"));
            }
            catch (Exception ex)
            {
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