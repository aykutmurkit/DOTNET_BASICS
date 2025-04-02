using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Kullanıcı repository arayüzü
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Tüm kullanıcıları getirir
        /// </summary>
        Task<List<User>> GetAllUsersAsync();
        
        /// <summary>
        /// ID'ye göre kullanıcı getirir
        /// </summary>
        Task<User> GetUserByIdAsync(int id);
        
        /// <summary>
        /// Kullanıcı adına göre kullanıcı getirir
        /// </summary>
        Task<User> GetUserByUsernameAsync(string username);
        
        /// <summary>
        /// E-postaya göre kullanıcı getirir
        /// </summary>
        Task<User> GetUserByEmailAsync(string email);
        
        /// <summary>
        /// Rol ID'sine göre kullanıcıları getirir
        /// </summary>
        Task<List<User>> GetUsersByRoleIdAsync(int roleId);
        
        /// <summary>
        /// Kullanıcı ekler
        /// </summary>
        Task AddUserAsync(User user);
        
        /// <summary>
        /// Kullanıcı günceller
        /// </summary>
        Task UpdateUserAsync(User user);
        
        /// <summary>
        /// Kullanıcı siler
        /// </summary>
        Task DeleteUserAsync(int id);
        
        /// <summary>
        /// Kullanıcı adının zaten kullanılıp kullanılmadığını kontrol eder
        /// </summary>
        Task<bool> UsernameExistsAsync(string username);
        
        /// <summary>
        /// E-postanın zaten kullanılıp kullanılmadığını kontrol eder
        /// </summary>
        Task<bool> EmailExistsAsync(string email);
    }
} 