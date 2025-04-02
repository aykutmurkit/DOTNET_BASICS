using AuthenticationApi.Business.Services.Interfaces;
using AuthenticationApi.Core.Logging;
using Core.Utilities;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using static Core.Utilities.ControllerExtensions;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogService _logService;

        public UsersController(IUserService userService, ILogService logService)
        {
            _userService = userService;
            _logService = logService;
        }

        /// <summary>
        /// Tüm kullanıcıları getirir
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "GetAllUsers", null);
                
                var users = await _userService.GetAllUsersAsync();
                var response = ApiResponse<List<UserDto>>.Success(users, "Kullanıcılar başarıyla getirildi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "GetAllUsers", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "GetAllUsers", ex);
                
                return StatusCode(500, ApiResponse<List<UserDto>>.ServerError(ex.Message));
            }
        }

        /// <summary>
        /// ID'ye göre kullanıcı getirir
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(int id)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "GetUser", new { id });
                
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && currentUserId != id)
                {
                    var forbiddenResponse = ApiResponse<UserDto>.Forbidden("Başka bir kullanıcının bilgilerini görüntüleme yetkiniz yok");
                    _logService.LogResponse(HttpContext, "Users", "GetUser", forbiddenResponse, 403);
                    return StatusCode(403, forbiddenResponse);
                }

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    var notFoundResponse = ApiResponse<UserDto>.NotFound($"ID: {id} olan kullanıcı bulunamadı");
                    _logService.LogResponse(HttpContext, "Users", "GetUser", notFoundResponse, 404);
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<UserDto>.Success(user, "Kullanıcı başarıyla getirildi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "GetUser", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "GetUser", ex);
                
                return StatusCode(500, ApiResponse<UserDto>.ServerError(ex.Message));
            }
        }

        /// <summary>
        /// Oturum açan kullanıcının profilini getirir
        /// </summary>
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "GetUserProfile", null);
                
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _userService.GetUserByIdAsync(userId);
                
                if (user == null)
                {
                    var notFoundResponse = ApiResponse<UserDto>.NotFound("Kullanıcı profili bulunamadı");
                    _logService.LogResponse(HttpContext, "Users", "GetUserProfile", notFoundResponse, 404);
                    return NotFound(notFoundResponse);
                }
                
                var response = ApiResponse<UserDto>.Success(user, "Profil başarıyla getirildi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "GetUserProfile", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "GetUserProfile", ex);
                
                return StatusCode(500, ApiResponse<UserDto>.ServerError(ex.Message));
            }
        }

        /// <summary>
        /// Kullanıcı oluşturur
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "CreateUser", request);
                
                var user = await _userService.CreateUserAsync(request);
                var response = ApiResponse<UserDto>.Created(user, "Kullanıcı başarıyla oluşturuldu");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "CreateUser", response, 201);
                
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "CreateUser", ex);
                
                if (ex.Message.Contains("zaten kullanılıyor"))
                {
                    return StatusCode(409, ApiResponse<UserDto>.Conflict(ex.Message));
                }
                return BadRequest(ApiResponse<UserDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Kullanıcı günceller
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "UpdateUser", new { id, request });
                
                var user = await _userService.UpdateUserAsync(id, request);
                var response = ApiResponse<UserDto>.Success(user, "Kullanıcı başarıyla güncellendi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "UpdateUser", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "UpdateUser", ex);
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<UserDto>.NotFound(ex.Message));
                }
                if (ex.Message.Contains("zaten kullanılıyor"))
                {
                    return StatusCode(409, ApiResponse<UserDto>.Conflict(ex.Message));
                }
                return BadRequest(ApiResponse<UserDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Kullanıcı siler
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "DeleteUser", new { id });
                
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    var notFoundResponse = ApiResponse<object>.NotFound("Kullanıcı bulunamadı");
                    _logService.LogResponse(HttpContext, "Users", "DeleteUser", notFoundResponse, 404);
                    return NotFound(notFoundResponse);
                }

                // Kendini silmeye çalışan admin kontrolü
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (id == currentUserId)
                {
                    var forbiddenResponse = ApiResponse<object>.Forbidden("Kendi kullanıcı hesabınızı silemezsiniz");
                    _logService.LogResponse(HttpContext, "Users", "DeleteUser", forbiddenResponse, 403);
                    return StatusCode(403, forbiddenResponse);
                }

                await _userService.DeleteUserAsync(id);
                var response = ApiResponse<object>.NoContent("Kullanıcı başarıyla silindi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "DeleteUser", response, 204);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "DeleteUser", ex);
                
                return StatusCode(500, ApiResponse<object>.ServerError(ex.Message));
            }
        }

        /// <summary>
        /// Kullanıcının kendi profilini günceller
        /// </summary>
        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile([FromBody] UpdateUserRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "UpdateUserProfile", request);
                
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                // Güvenlik kontrolü: Kullanıcı kendi rolünü değiştiremez
                if (request.RoleId.HasValue)
                {
                    var forbiddenResponse = ApiResponse<UserDto>.Forbidden("Rolünüzü değiştiremezsiniz");
                    _logService.LogResponse(HttpContext, "Users", "UpdateUserProfile", forbiddenResponse, 403);
                    return StatusCode(403, forbiddenResponse);
                }
                
                var user = await _userService.UpdateUserAsync(userId, request);
                var response = ApiResponse<UserDto>.Success(user, "Profil başarıyla güncellendi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "UpdateUserProfile", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "UpdateUserProfile", ex);
                
                if (ex.Message.Contains("zaten kullanılıyor"))
                {
                    return StatusCode(409, ApiResponse<UserDto>.Conflict(ex.Message));
                }
                return BadRequest(ApiResponse<UserDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Profil fotoğrafı yükler
        /// </summary>
        [HttpPost("profile-picture")]
        [EnableRateLimiting("api_users_profile-picture")]
        public async Task<ActionResult<ApiResponse<object>>> UploadProfilePicture([FromForm] UploadProfilePictureRequest request)
        {
            try
            {
                // İstek logla (dosya içeriği hariç)
                _logService.LogRequest(HttpContext, "Users", "UploadProfilePicture", new { FileName = request.ProfilePicture?.FileName, FileSize = request.ProfilePicture?.Length });
                
                if (request.ProfilePicture == null || request.ProfilePicture.Length == 0)
                {
                    var badRequestResponse = ApiResponse<object>.Error("Profil resmi dosyası gereklidir");
                    _logService.LogResponse(HttpContext, "Users", "UploadProfilePicture", badRequestResponse, 400);
                    return BadRequest(badRequestResponse);
                }

                if (request.ProfilePicture.Length > 1024 * 1024) // 1MB
                {
                    var badRequestResponse = ApiResponse<object>.Error("Profil resmi dosyası 1MB'dan küçük olmalıdır");
                    _logService.LogResponse(HttpContext, "Users", "UploadProfilePicture", badRequestResponse, 400);
                    return BadRequest(badRequestResponse);
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(request.ProfilePicture.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    var badRequestResponse = ApiResponse<object>.Error("Sadece jpg, jpeg ve png uzantılı dosyalar kabul edilmektedir");
                    _logService.LogResponse(HttpContext, "Users", "UploadProfilePicture", badRequestResponse, 400);
                    return BadRequest(badRequestResponse);
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                using var stream = new MemoryStream();
                await request.ProfilePicture.CopyToAsync(stream);

                await _userService.UploadProfilePictureAsync(userId, request);
                var response = ApiResponse<object>.Success(null, "Profil fotoğrafı başarıyla yüklendi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "UploadProfilePicture", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "UploadProfilePicture", ex);
                
                return StatusCode(500, ApiResponse<object>.ServerError(ex.Message));
            }
        }

        /// <summary>
        /// Kullanıcının profil fotoğrafını getirir
        /// </summary>
        [HttpGet("{id}/profile-picture")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfilePicture(int id)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "GetProfilePicture", new { id });
                
                var profilePicture = await _userService.GetProfilePictureAsync(id);
                if (profilePicture == null)
                {
                    // Varsayılan profil resmi döndür
                    var defaultPicture = await _userService.GetProfilePictureAsync(0); // 0 ID ile varsayılan resim isteği
                    
                    // Yanıt logla (başarılı ama varsayılan resim)
                    _logService.LogResponse(HttpContext, "Users", "GetProfilePicture", "Default profile picture returned", 200);
                    
                    return File(defaultPicture, "image/png");
                }
                
                // Yanıt logla (başarılı)
                _logService.LogResponse(HttpContext, "Users", "GetProfilePicture", "Profile picture returned", 200);
                
                return File(profilePicture, "image/png");
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "GetProfilePicture", ex);
                
                // Varsayılan profil resmi döndür (hata durumunda)
                try
                {
                    var defaultPicture = await _userService.GetProfilePictureAsync(0); // 0 ID ile varsayılan resim isteği
                    return File(defaultPicture, "image/png");
                }
                catch
                {
                    return StatusCode(500, "Profil resmi yüklenirken bir hata oluştu");
                }
            }
        }

        /// <summary>
        /// Random şifre ile kullanıcı oluşturur
        /// </summary>
        [HttpPost("random-password")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUserWithRandomPassword([FromBody] RandomPasswordUserRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "CreateUserWithRandomPassword", request);
                
                var user = await _userService.CreateUserWithRandomPasswordAsync(request);
                var response = ApiResponse<UserDto>.Created(user, "Kullanıcı otomatik şifre ile başarıyla oluşturuldu ve e-posta gönderildi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "CreateUserWithRandomPassword", response, 201);
                
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "CreateUserWithRandomPassword", ex);
                
                if (ex.Message.Contains("zaten kullanılıyor"))
                {
                    return StatusCode(409, ApiResponse<UserDto>.Conflict(ex.Message));
                }
                return BadRequest(ApiResponse<UserDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Sadece kullanıcı rolünü günceller
        /// </summary>
        [HttpPatch("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUserRole(int id, [FromBody] UpdateUserRoleRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "UpdateUserRole", new { id, request });
                
                var user = await _userService.UpdateUserRoleAsync(id, request);
                var response = ApiResponse<UserDto>.Success(user, "Kullanıcı rolü başarıyla güncellendi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "UpdateUserRole", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "UpdateUserRole", ex);
                
                if (ex.Message.Contains("rolü bulunamadı"))
                {
                    return NotFound(ApiResponse<UserDto>.NotFound(ex.Message));
                }
                
                return BadRequest(ApiResponse<UserDto>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Sadece kullanıcı e-posta adresini günceller
        /// </summary>
        [HttpPatch("{id}/email")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUserEmail(int id, [FromBody] UpdateUserEmailRequest request)
        {
            try
            {
                // İstek logla
                _logService.LogRequest(HttpContext, "Users", "UpdateUserEmail", new { id, request });
                
                var user = await _userService.UpdateUserEmailAsync(id, request);
                var response = ApiResponse<UserDto>.Success(user, "Kullanıcı e-posta adresi başarıyla güncellendi");
                
                // Yanıt logla
                _logService.LogResponse(HttpContext, "Users", "UpdateUserEmail", response, 200);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata logla
                _logService.LogException(HttpContext, "Users", "UpdateUserEmail", ex);
                
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<UserDto>.NotFound(ex.Message));
                }
                if (ex.Message.Contains("zaten kullanılıyor"))
                {
                    return StatusCode(409, ApiResponse<UserDto>.Conflict(ex.Message));
                }
                return BadRequest(ApiResponse<UserDto>.Error(ex.Message));
            }
        }
    }
} 