using AuthApi.Business.Services.Interfaces;
using Core.Utilities;
using Entities.Dtos;
using LogLibrary.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using static Core.Utilities.ControllerExtensions;

namespace AuthApi.Controllers
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
            await _logService.LogInfoAsync(
                message: "Tüm kullanıcılar listeleniyor", 
                source: "UsersController.GetAllUsers",
                userId: User.FindFirstValue(ClaimTypes.NameIdentifier),
                userName: User.FindFirstValue(ClaimTypes.Name));
                
            try
            {
                var users = await _userService.GetAllUsersAsync();
                
                await _logService.LogInfoAsync(
                    message: $"{users.Count} kullanıcı listelendi", 
                    source: "UsersController.GetAllUsers",
                    data: new { Count = users.Count },
                    userId: User.FindFirstValue(ClaimTypes.NameIdentifier),
                    userName: User.FindFirstValue(ClaimTypes.Name));
                    
                return Ok(ApiResponse<List<UserDto>>.Success(users, "Kullanıcılar başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    message: "Kullanıcı listeleme hatası", 
                    source: "UsersController.GetAllUsers",
                    exception: ex,
                    userId: User.FindFirstValue(ClaimTypes.NameIdentifier),
                    userName: User.FindFirstValue(ClaimTypes.Name));
                    
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
            await _logService.LogInfoAsync(
                message: $"ID: {id} olan kullanıcı sorgulanıyor", 
                source: "UsersController.GetUserById",
                data: new { UserId = id },
                userId: User.FindFirstValue(ClaimTypes.NameIdentifier),
                userName: User.FindFirstValue(ClaimTypes.Name));
                
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    await _logService.LogWarningAsync(
                        message: $"ID: {id} olan kullanıcı bulunamadı", 
                        source: "UsersController.GetUserById",
                        userId: User.FindFirstValue(ClaimTypes.NameIdentifier),
                        userName: User.FindFirstValue(ClaimTypes.Name));
                        
                    return NotFound(ApiResponse<UserDto>.NotFound($"ID: {id} olan kullanıcı bulunamadı"));
                }

                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && currentUserId != id)
                {
                    await _logService.LogWarningAsync(
                        message: $"Yetkisiz kullanıcı erişim denemesi - ID: {currentUserId}, Erişim istenilen ID: {id}", 
                        source: "UsersController.GetUserById",
                        userId: currentUserId.ToString(),
                        userName: User.FindFirstValue(ClaimTypes.Name));
                        
                    return StatusCode(403, ApiResponse<UserDto>.Forbidden("Başka bir kullanıcının bilgilerini görüntüleme yetkiniz yok"));
                }

                await _logService.LogInfoAsync(
                    message: $"ID: {id} olan kullanıcı başarıyla getirildi", 
                    source: "UsersController.GetUserById",
                    userId: User.FindFirstValue(ClaimTypes.NameIdentifier),
                    userName: User.FindFirstValue(ClaimTypes.Name));
                    
                return Ok(ApiResponse<UserDto>.Success(user, "Kullanıcı başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    message: $"ID: {id} olan kullanıcıyı getirme hatası", 
                    source: "UsersController.GetUserById",
                    exception: ex,
                    userId: User.FindFirstValue(ClaimTypes.NameIdentifier),
                    userName: User.FindFirstValue(ClaimTypes.Name));
                    
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
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(ApiResponse<UserDto>.Success(user, "Profil başarıyla getirildi"));
            }
            catch (Exception ex)
            {
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
                var user = await _userService.CreateUserAsync(request);
                var response = ApiResponse<UserDto>.Created(user, "Kullanıcı başarıyla oluşturuldu");
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
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
                var user = await _userService.UpdateUserAsync(id, request);
                return Ok(ApiResponse<UserDto>.Success(user, "Kullanıcı başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
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
                await _userService.DeleteUserAsync(id);
                var response = ApiResponse<object>.NoContent("Kullanıcı başarıyla silindi");
                return StatusCode(204, response);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("bulunamadı"))
                {
                    return NotFound(ApiResponse<object>.NotFound(ex.Message));
                }
                return BadRequest(ApiResponse<object>.Error(ex.Message));
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
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                // Güvenlik kontrolü: Kullanıcı kendi rolünü değiştiremez
                if (request.RoleId.HasValue)
                {
                    return StatusCode(403, ApiResponse<UserDto>.Forbidden("Rolünüzü değiştiremezsiniz"));
                }
                
                var user = await _userService.UpdateUserAsync(userId, request);
                return Ok(ApiResponse<UserDto>.Success(user, "Profil başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            
            await _logService.LogInfoAsync(
                message: "Profil fotoğrafı yükleme isteği", 
                source: "UsersController.UploadProfilePicture",
                data: new { FileSize = request.File?.Length, FileName = request.File?.FileName },
                userId: userId,
                userName: userName);
                
            try
            {
                if (request.File == null || request.File.Length == 0)
                {
                    await _logService.LogWarningAsync(
                        message: "Boş profil fotoğrafı yükleme denemesi", 
                        source: "UsersController.UploadProfilePicture",
                        userId: userId,
                        userName: userName);
                        
                    return BadRequest(ApiResponse<object>.Error("Geçerli bir dosya seçiniz"));
                }

                var userIdFromClaim = int.Parse(userId);
                await _userService.UpdateProfilePictureAsync(userIdFromClaim, request.File);
                
                await _logService.LogInfoAsync(
                    message: "Profil fotoğrafı başarıyla yüklendi", 
                    source: "UsersController.UploadProfilePicture",
                    data: new { FileSize = request.File.Length, FileName = request.File.FileName },
                    userId: userId,
                    userName: userName);
                    
                return Ok(ApiResponse<object>.Success(null, "Profil fotoğrafı başarıyla yüklendi"));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync(
                    message: "Profil fotoğrafı yükleme hatası", 
                    source: "UsersController.UploadProfilePicture",
                    exception: ex,
                    userId: userId,
                    userName: userName);
                    
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
                var profilePicture = await _userService.GetProfilePictureAsync(id);
                if (profilePicture == null)
                {
                    return NotFound();
                }

                return File(profilePicture, "image/png");
            }
            catch (Exception ex)
            {
                return NotFound();
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
                var user = await _userService.CreateUserWithRandomPasswordAsync(request);
                var response = ApiResponse<UserDto>.Created(user, "Kullanıcı otomatik şifre ile başarıyla oluşturuldu ve e-posta gönderildi");
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
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
                var user = await _userService.UpdateUserRoleAsync(id, request);
                return Ok(ApiResponse<UserDto>.Success(user, "Kullanıcı rolü başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("bulunamadı"))
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
                var user = await _userService.UpdateUserEmailAsync(id, request);
                return Ok(ApiResponse<UserDto>.Success(user, "Kullanıcı e-posta adresi başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
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