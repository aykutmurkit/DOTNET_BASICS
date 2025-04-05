using DeviceApi.Business.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DeviceApi.Business.Services.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(string Token, DateTime Expiration)> GenerateAccessTokenAsync(int userId, string username, string email, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationMinutes = int.Parse(jwtSettings["AccessTokenExpirationInMinutes"]);
            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role)
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);
            
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);
            
            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }

        public async Task<(string Token, DateTime Expiration)> GenerateRefreshTokenAsync(int userId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationDays = int.Parse(jwtSettings["RefreshTokenExpirationInDays"]);
            
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(expirationDays);
            
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);
            
            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // We don't care about the token's expiration when validating refresh
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            
            return principal;
        }

        // For demo purposes, hardcoded user validation 
        // In a real application, this would validate against a database
        public Task<bool> ValidateUserAsync(string username, string password)
        {
            // Just for demonstration - replace with actual DB validation in real app
            return Task.FromResult(username == "aykut" && password == "Password123!");
        }

        // For demo purposes, hardcoded user info
        // In a real application, this would query the database
        public Task<(int UserId, string Username, string Email, string Role)> GetUserInfoAsync(string username)
        {
            // Just for demonstration - replace with actual DB lookup in real app
            if (username == "aykut")
            {
                return Task.FromResult((4, "aykut", "aykutmurkit.dev@gmail.com", "Admin"));
            }
            
            throw new ArgumentException("User not found");
        }
    }
} 