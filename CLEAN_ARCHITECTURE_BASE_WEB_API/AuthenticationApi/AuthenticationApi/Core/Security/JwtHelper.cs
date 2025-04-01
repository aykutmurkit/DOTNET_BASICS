using Entities.Concrete;
using Entities.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Data.Context;

namespace Core.Security
{
    /// <summary>
    /// JWT token işlemleri yardımcı sınıfı
    /// </summary>
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public JwtHelper(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Erişim tokeni oluşturur
        /// </summary>
        public string GenerateAccessToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);
            
            // Rol adını getir
            var roleName = _context.UserRoles
                .Where(r => r.Id == user.RoleId)
                .Select(r => r.Name)
                .FirstOrDefault() ?? "Unknown";
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Yenileme tokeni oluşturur
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Tokenden kullanıcı kimlik bilgilerini çıkarır
        /// </summary>
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Süre kontrolü yapma çünkü refresh token için kullanılıyor
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Kimlik doğrulama yanıtı oluşturur
        /// </summary>
        public AuthResponse CreateAuthResponse(User user, string accessToken, string refreshToken)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationInMinutes"]));
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["RefreshTokenExpirationInDays"]));
            
            // Rol adını getir
            var roleName = _context.UserRoles
                .Where(r => r.Id == user.RoleId)
                .Select(r => r.Name)
                .FirstOrDefault() ?? "Unknown";
            
            return new AuthResponse
            {
                AccessToken = new TokenInfo
                {
                    Token = accessToken,
                    ExpiresAt = accessTokenExpiresAt
                },
                RefreshToken = new TokenInfo
                {
                    Token = refreshToken,
                    ExpiresAt = refreshTokenExpiresAt
                },
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = new RoleInfo
                    {
                        Id = user.RoleId,
                        Name = roleName
                    }
                }
            };
        }
    }
} 