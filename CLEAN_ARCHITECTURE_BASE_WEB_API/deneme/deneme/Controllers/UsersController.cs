using Business.Interfaces;
using Core.Utilities;
using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Core.Utilities.ControllerExtensions;

namespace deneme.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
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
                var users = await _userService.GetAllUsersAsync();
                return Ok(ApiResponse<List<UserDto>>.Success(users, "Kullanıcılar başarıyla getirildi"));
            }
            catch (Exception ex)
            {
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
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.NotFound($"ID: {id} olan kullanıcı bulunamadı"));
                }

                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && currentUserId != id)
                {
                    return StatusCode(403, ApiResponse<UserDto>.Forbidden("Başka bir kullanıcının bilgilerini görüntüleme yetkiniz yok"));
                }

                return Ok(ApiResponse<UserDto>.Success(user, "Kullanıcı başarıyla getirildi"));
            }
            catch (Exception ex)
            {
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
        public async Task<ActionResult<ApiResponse<object>>> UploadProfilePicture([FromForm] UploadProfilePictureRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _userService.UploadProfilePictureAsync(userId, request);
                
                // Profil fotoğrafı yüklendikten sonra güncel kullanıcı bilgilerini döndür
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(ApiResponse<UserDto>.Success(user, "Profil fotoğrafı başarıyla yüklendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Profil fotoğrafını getirir
        /// </summary>
        [HttpGet("{id}/profile-picture")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfilePicture(int id)
        {
            try
            {
                var profilePicture = await _userService.GetProfilePictureAsync(id);
                return File(profilePicture, "image/png");
            }
            catch (Exception ex)
            {
                // Profil fotoğrafı durumunda ApiResponse yerine doğrudan dosya dönüyoruz,
                // hatalı durumda ise normal bir hata yanıtı
                return NotFound(ApiResponse<object>.NotFound(ex.Message));
            }
        }
    }
} 