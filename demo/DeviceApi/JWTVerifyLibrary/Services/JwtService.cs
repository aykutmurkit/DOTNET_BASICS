using JWTVerifyLibrary.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTVerifyLibrary.Services
{
    public interface IJwtService
    {
        bool ValidateToken(string token, out JwtSecurityToken? validatedToken);
        IEnumerable<Claim> GetTokenClaims(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero
            };
        }

        public bool ValidateToken(string token, out JwtSecurityToken? validatedToken)
        {
            validatedToken = null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                
                tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken securityToken);
                
                if (securityToken is JwtSecurityToken jwtSecurityToken && 
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    validatedToken = jwtSecurityToken;
                    return true;
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            if (ValidateToken(token, out JwtSecurityToken? validatedToken) && validatedToken != null)
            {
                return validatedToken.Claims;
            }

            return Enumerable.Empty<Claim>();
        }
    }
} 