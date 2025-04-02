using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
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
        /// Random şifre ile kullanıcı oluşturur
        /// </summary>
        Task<UserDto> CreateUserWithRandomPasswordAsync(RandomPasswordUserRequest request);

        /// <summary>
        /// Kullanıcı günceller
        /// </summary>
        Task<UserDto> UpdateUserAsync(int id, UpdateUserRequest request);

        /// <summary>
        /// Kullanıcı rolünü günceller
        /// </summary>
        Task<UserDto> UpdateUserRoleAsync(int id, UpdateUserRoleRequest request);

        /// <summary>
        /// Kullanıcı e-posta adresini günceller
        /// </summary>
        Task<UserDto> UpdateUserEmailAsync(int id, UpdateUserEmailRequest request);

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