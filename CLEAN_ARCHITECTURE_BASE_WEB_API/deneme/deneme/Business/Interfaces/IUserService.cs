using Entities.Concrete;
using Entities.Dtos;

namespace Business.Interfaces
{
    /// <summary>
    /// Kullanıcı servis arayüzü
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Tüm kullanıcıları getirir
        /// </summary>
        Task<List<UserDto>> GetAllUsersAsync();
        
        /// <summary>
        /// ID'ye göre kullanıcı getirir
        /// </summary>
        Task<UserDto> GetUserByIdAsync(int id);
        
        /// <summary>
        /// Kullanıcı oluşturur
        /// </summary>
        Task<UserDto> CreateUserAsync(CreateUserRequest request);
        
        /// <summary>
        /// Kullanıcı günceller
        /// </summary>
        Task<UserDto> UpdateUserAsync(int id, UpdateUserRequest request);
        
        /// <summary>
        /// Kullanıcı siler
        /// </summary>
        Task DeleteUserAsync(int id);
        
        /// <summary>
        /// Profil fotoğrafı yükler
        /// </summary>
        Task UploadProfilePictureAsync(int userId, UploadProfilePictureRequest request);
        
        /// <summary>
        /// Profil fotoğrafını getirir
        /// </summary>
        Task<byte[]> GetProfilePictureAsync(int userId);
    }
} 