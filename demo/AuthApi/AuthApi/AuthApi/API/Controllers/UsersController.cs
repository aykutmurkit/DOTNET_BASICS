using AuthApi.Business.Services.Interfaces;
using Core.Utilities;
using Entities.Dtos;
using LogLib.Services;
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
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                await _logService.LogInfoAsync(
                    "Retrieving all users", 
                    userId, 
                    username, 
                    userEmail);
                
                var users = await _userService.GetAllUsersAsync();
                
                await _logService.LogInfoAsync(
                    $"Successfully retrieved {users.Count} users", 
                    userId, 
                    username, 
                    userEmail);
                    
                return Ok(ApiResponse<List<UserDto>>.Success(users, "Kullanıcılar başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                await _logService.LogErrorAsync(
                    "Error retrieving all users", 
                    ex, 
                    userId, 
                    username, 
                    userEmail);
                    
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                await _logService.LogInfoAsync(
                    $"Retrieving user with ID: {id}", 
                    userId, 
                    username, 
                    userEmail);
                
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    await _logService.LogWarningAsync(
                        $"User with ID: {id} not found", 
                        userId, 
                        username, 
                        userEmail);
                        
                    return NotFound(ApiResponse<UserDto>.NotFound($"ID: {id} olan kullanıcı bulunamadı"));
                }

                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && currentUserId != id)
                {
                    await _logService.LogSecurityAsync(
                        $"Unauthorized attempt to access user with ID: {id} by user: {username}", 
                        userId, 
                        username, 
                        userEmail, 
                        HttpContext.Connection.RemoteIpAddress?.ToString());
                        
                    return StatusCode(403, ApiResponse<UserDto>.Forbidden("Başka bir kullanıcının bilgilerini görüntüleme yetkiniz yok"));
                }

                await _logService.LogInfoAsync(
                    $"Successfully retrieved user with ID: {id}", 
                    userId, 
                    username, 
                    userEmail);
                
                return Ok(ApiResponse<UserDto>.Success(user, "Kullanıcı başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                await _logService.LogErrorAsync(
                    $"Error retrieving user with ID: {id}", 
                    ex, 
                    userId, 
                    username, 
                    userEmail);
                    
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                await _logService.LogInfoAsync(
                    "User retrieving own profile", 
                    userId, 
                    username, 
                    userEmail);
                    
                var userIdInt = int.Parse(userId);
                var user = await _userService.GetUserByIdAsync(userIdInt);
                
                await _logService.LogInfoAsync(
                    "Successfully retrieved user profile", 
                    userId, 
                    username, 
                    userEmail);
                    
                return Ok(ApiResponse<UserDto>.Success(user, "Profil başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                await _logService.LogErrorAsync(
                    "Error retrieving user profile", 
                    ex, 
                    userId, 
                    username, 
                    userEmail);
                    
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                var additionalData = new Dictionary<string, object>
                {
                    { "NewUsername", request.Username },
                    { "NewUserEmail", request.Email },
                    { "RoleId", request.RoleId }
                };
                
                await _logService.LogInfoAsync(
                    $"Creating new user with username: {request.Username}", 
                    userId, 
                    username, 
                    userEmail, 
                    null, 
                    additionalData);
                    
                var user = await _userService.CreateUserAsync(request);
                
                await _logService.LogInfoAsync(
                    $"Successfully created user with ID: {user.Id}, username: {user.Username}", 
                    userId, 
                    username, 
                    userEmail);
                    
                var response = ApiResponse<UserDto>.Created(user, "Kullanıcı başarıyla oluşturuldu");
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                await _logService.LogErrorAsync(
                    $"Error creating user with username: {request.Username}", 
                    ex, 
                    userId, 
                    username, 
                    userEmail);
                    
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                var additionalData = new Dictionary<string, object>
                {
                    { "UserId", id },
                    { "UpdatedFields", string.Join(", ", typeof(UpdateUserRequest).GetProperties().Select(p => p.Name)) }
                };
                
                await _logService.LogInfoAsync(
                    $"Updating user with ID: {id}", 
                    userId, 
                    username, 
                    userEmail, 
                    null, 
                    additionalData);
                    
                var user = await _userService.UpdateUserAsync(id, request);
                
                await _logService.LogInfoAsync(
                    $"Successfully updated user with ID: {id}", 
                    userId, 
                    username, 
                    userEmail);
                    
                return Ok(ApiResponse<UserDto>.Success(user, "Kullanıcı başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                await _logService.LogErrorAsync(
                    $"Error updating user with ID: {id}", 
                    ex, 
                    userId, 
                    username, 
                    userEmail);
                    
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                await _logService.LogWarningAsync(
                    $"Deleting user with ID: {id}", 
                    userId, 
                    username, 
                    userEmail);
                    
                await _userService.DeleteUserAsync(id);
                
                await _logService.LogInfoAsync(
                    $"Successfully deleted user with ID: {id}", 
                    userId, 
                    username, 
                    userEmail);
                    
                var response = ApiResponse<object>.NoContent("Kullanıcı başarıyla silindi");
                return StatusCode(204, response);
            }
            catch (Exception ex)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                
                await _logService.LogErrorAsync(
                    $"Error deleting user with ID: {id}", 
                    ex, 
                    userId, 
                    username, 
                    userEmail);
                    
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
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _userService.UploadProfilePictureAsync(userId, request);
                return Ok(ApiResponse<object>.Success(null, "Profil fotoğrafı başarıyla yüklendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
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