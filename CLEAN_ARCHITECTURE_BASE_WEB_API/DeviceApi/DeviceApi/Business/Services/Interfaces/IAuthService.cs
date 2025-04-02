using System.Security.Claims;

namespace DeviceApi.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(string Token, DateTime Expiration)> GenerateAccessTokenAsync(int userId, string username, string email, string role);
        Task<(string Token, DateTime Expiration)> GenerateRefreshTokenAsync(int userId);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<bool> ValidateUserAsync(string username, string password);
        Task<(int UserId, string Username, string Email, string Role)> GetUserInfoAsync(string username);
    }
} 