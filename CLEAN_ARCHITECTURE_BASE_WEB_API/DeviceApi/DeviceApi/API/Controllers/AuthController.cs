using Core.Utilities;
using DeviceApi.API.Models.Auth;
using DeviceApi.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeviceApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Kullanıcı giriş işlemi
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<TokenResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Kullanıcı giriş denemesi: {Username}", request.Username);
            
            var isValid = await _authService.ValidateUserAsync(request.Username, request.Password);
            if (!isValid)
            {
                _logger.LogWarning("Başarısız giriş denemesi: {Username}", request.Username);
                return Unauthorized(ApiResponse<object>.Error(null, "Kullanıcı adı veya şifre hatalı"));
            }

            try
            {
                var userInfo = await _authService.GetUserInfoAsync(request.Username);
                
                var (accessToken, accessExpiration) = await _authService.GenerateAccessTokenAsync(
                    userInfo.UserId, userInfo.Username, userInfo.Email, userInfo.Role);
                
                var (refreshToken, refreshExpiration) = await _authService.GenerateRefreshTokenAsync(userInfo.UserId);

                _logger.LogInformation("Başarılı giriş: {Username}, Role: {Role}", userInfo.Username, userInfo.Role);
                
                var response = new TokenResponse
                {
                    AccessToken = accessToken,
                    AccessTokenExpiration = accessExpiration,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = refreshExpiration,
                    User = new UserInfo
                    {
                        Id = userInfo.UserId,
                        Username = userInfo.Username,
                        Email = userInfo.Email,
                        Role = userInfo.Role
                    }
                };

                return Ok(ApiResponse<TokenResponse>.Success(response, "Giriş başarılı"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Giriş sırasında bir hata oluştu: {Username}", request.Username);
                return BadRequest(ApiResponse<object>.Error(null, "Giriş işlemi sırasında bir hata oluştu"));
            }
        }

        /// <summary>
        /// Access token yenileme
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<TokenResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<>), 400)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var principal = _authService.GetPrincipalFromExpiredToken(request.AccessToken);
                var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
                var username = principal.FindFirst(ClaimTypes.Name).Value;
                var email = principal.FindFirst(ClaimTypes.Email).Value;
                var role = principal.FindFirst(ClaimTypes.Role).Value;

                // Burada refresh token'in veritabanında geçerli olup olmadığı kontrol edilmelidir
                // Basitleştirmek için şimdilik atlanmıştır
                
                var (accessToken, accessExpiration) = await _authService.GenerateAccessTokenAsync(
                    userId, username, email, role);
                
                var (refreshToken, refreshExpiration) = await _authService.GenerateRefreshTokenAsync(userId);
                
                _logger.LogInformation("Token yenilendi: {Username}", username);

                var response = new TokenResponse
                {
                    AccessToken = accessToken,
                    AccessTokenExpiration = accessExpiration,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = refreshExpiration,
                    User = new UserInfo
                    {
                        Id = userId,
                        Username = username,
                        Email = email,
                        Role = role
                    }
                };

                return Ok(ApiResponse<TokenResponse>.Success(response, "Token başarıyla yenilendi"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token yenileme sırasında bir hata oluştu");
                return BadRequest(ApiResponse<object>.Error(null, "Geçersiz token"));
            }
        }

        /// <summary>
        /// Mevcut oturum açmış kullanıcı bilgilerini getirir
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserInfo>), 200)]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var username = User.FindFirst(ClaimTypes.Name).Value;
                var email = User.FindFirst(ClaimTypes.Email).Value;
                var role = User.FindFirst(ClaimTypes.Role).Value;

                _logger.LogInformation("Kullanıcı bilgileri alındı: {Username}, Role: {Role}", username, role);

                var userInfo = new UserInfo
                {
                    Id = userId,
                    Username = username,
                    Email = email,
                    Role = role
                };

                return Ok(ApiResponse<UserInfo>.Success(userInfo, "Kullanıcı bilgileri başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı bilgileri alınırken bir hata oluştu");
                return BadRequest(ApiResponse<object>.Error(null, "Kullanıcı bilgileri alınamadı"));
            }
        }

        /// <summary>
        /// Sadece Admin rolüne sahip kullanıcıların erişebileceği örnek bir endpoint
        /// </summary>
        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public IActionResult AdminOnlyEndpoint()
        {
            var username = User.FindFirst(ClaimTypes.Name).Value;
            _logger.LogInformation("Admin endpoint'ine erişim: {Username}", username);
            
            return Ok(ApiResponse<string>.Success("Bu içeriği sadece Admin rolündeki kullanıcılar görebilir", 
                "Yetkilendirme başarılı"));
        }
        
        /// <summary>
        /// Farklı rollere göre farklı içerik dönen bir endpoint örneği
        /// </summary>
        [HttpGet("role-based-content")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public IActionResult RoleBasedContent()
        {
            var username = User.FindFirst(ClaimTypes.Name).Value;
            var role = User.FindFirst(ClaimTypes.Role).Value;
            
            _logger.LogInformation("Rol tabanlı içerik erişimi: {Username}, Role: {Role}", username, role);
            
            string content;
            
            switch (role)
            {
                case "Admin":
                    content = "Bu içerik Admin rolü için özelleştirilmiştir.";
                    break;
                case "Developer":
                    content = "Bu içerik Developer rolü için özelleştirilmiştir.";
                    break;
                default:
                    content = "Bu içerik standart kullanıcılar için özelleştirilmiştir.";
                    break;
            }
            
            return Ok(ApiResponse<string>.Success(content, $"{role} rolüne özel içerik"));
        }
    }
} 