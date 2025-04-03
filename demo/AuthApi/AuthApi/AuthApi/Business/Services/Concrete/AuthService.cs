using AuthApi.Business.Services.Interfaces;
using Core.Security;
using Core.Utilities;
using Data.Context;
using Data.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace AuthApi.Business.Services.Concrete
{
    /// <summary>
    /// Kimlik doğrulama servisi implementasyonu
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly EmailService _emailService;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthService(
            IUserRepository userRepository,
            JwtHelper jwtHelper,
            EmailService emailService,
            ITwoFactorService twoFactorService,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _twoFactorService = twoFactorService;
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Kullanıcı kayıt işlemi
        /// </summary>
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Kullanıcı adı ve e-posta kontrolü
            if (await _userRepository.UsernameExistsAsync(request.Username))
            {
                throw new Exception("Bu kullanıcı adı zaten kullanılıyor.");
            }

            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new Exception("Bu e-posta adresi zaten kullanılıyor.");
            }

            // Şifre hashleme
            string salt = PasswordHelper.CreateSalt();
            string passwordHash = PasswordHelper.HashPassword(request.Password, salt);

            // Varsayılan rol olarak User rolünü kullan (Id: 1)
            var userRole = await _context.UserRoles.FindAsync(1); // User role
            if (userRole == null)
            {
                throw new Exception("Varsayılan rol bulunamadı.");
            }

            // Yeni kullanıcı oluşturma
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                RoleId = userRole.Id,
                CreatedDate = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);

            // Token oluşturma
            string accessToken = _jwtHelper.GenerateAccessToken(user);
            string refreshToken = _jwtHelper.GenerateRefreshToken();

            // Refresh token kullanıcıya ekleme
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenExpirationInDays"]));
            await _userRepository.UpdateUserAsync(user);

            return _jwtHelper.CreateAuthResponse(user, accessToken, refreshToken);
        }

        /// <summary>
        /// Kullanıcı giriş işlemi
        /// </summary>
        public async Task<object> LoginAsync(LoginRequest request)
        {
            // Kullanıcıyı bulma
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (user == null)
            {
                throw new Exception("Kullanıcı adı veya şifre hatalı.");
            }

            // Şifre doğrulama
            if (!PasswordHelper.VerifyPassword(request.Password, user.PasswordSalt, user.PasswordHash))
            {
                throw new Exception("Kullanıcı adı veya şifre hatalı.");
            }

            // 2FA gerekli mi kontrol et
            if (_twoFactorService.IsTwoFactorRequiredForUser(user))
            {
                // 2FA kodu oluştur ve e-posta gönder
                string twoFactorCode = _twoFactorService.GenerateNewCodeForUser(user);
                await _userRepository.UpdateUserAsync(user);

                // E-posta şablonunu oku ve dinamik değerleri doldur
                string emailTemplate = File.ReadAllText("wwwroot/assets/templates/email/two-factor-auth.html");
                string emailHtml = emailTemplate
                    .Replace("{code}", twoFactorCode)
                    .Replace("{expirationMinutes}", user.TwoFactorCodeExpirationMinutes.ToString());

                // E-posta ile kodu gönder
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Doğrulama Kodunuz",
                    emailHtml);

                // 2FA gerektiğini belirten yanıt
                return new TwoFactorRequiredResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.Username
                };
            }

            // 2FA gerekmiyorsa normal giriş akışına devam et
            // Token oluşturma
            string accessToken = _jwtHelper.GenerateAccessToken(user);
            string refreshToken = _jwtHelper.GenerateRefreshToken();

            // Refresh token kullanıcıya ekleme
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenExpirationInDays"]));
            user.LastLoginDate = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);

            return _jwtHelper.CreateAuthResponse(user, accessToken, refreshToken);
        }

        /// <summary>
        /// İki faktörlü kimlik doğrulama aşaması
        /// </summary>
        public async Task<AuthResponse> VerifyTwoFactorAsync(TwoFactorVerifyRequest request)
        {
            // Kullanıcıyı bul
            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            // 2FA kodunu doğrula
            if (!_twoFactorService.ValidateCodeForUser(user, request.Code))
            {
                throw new Exception("Doğrulama kodu geçersiz veya süresi dolmuş.");
            }

            // Kodu temizle 
            _twoFactorService.ClearTwoFactorCodeForUser(user);

            // Token oluştur
            string accessToken = _jwtHelper.GenerateAccessToken(user);
            string refreshToken = _jwtHelper.GenerateRefreshToken();

            // Refresh token kullanıcıya ekleme
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenExpirationInDays"]));
            user.LastLoginDate = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);

            return _jwtHelper.CreateAuthResponse(user, accessToken, refreshToken);
        }

        /// <summary>
        /// Token yenileme işlemi
        /// </summary>
        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = await _userRepository.GetAllUsersAsync()
                .ContinueWith(t => t.Result.FirstOrDefault(u => u.RefreshToken == request.RefreshToken));

            if (user == null || user.RefreshTokenExpireDate <= DateTime.UtcNow)
            {
                throw new Exception("Geçersiz veya süresi dolmuş yenileme tokeni.");
            }

            // Yeni token oluşturma
            string accessToken = _jwtHelper.GenerateAccessToken(user);
            string refreshToken = _jwtHelper.GenerateRefreshToken();

            // Refresh token güncelleme
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(double.Parse(_configuration["JwtSettings:RefreshTokenExpirationInDays"]));
            await _userRepository.UpdateUserAsync(user);

            return _jwtHelper.CreateAuthResponse(user, accessToken, refreshToken);
        }

        /// <summary>
        /// Şifre değiştirme işlemi
        /// </summary>
        public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            // Mevcut şifre doğrulama
            if (!PasswordHelper.VerifyPassword(request.CurrentPassword, user.PasswordSalt, user.PasswordHash))
            {
                throw new Exception("Mevcut şifre hatalı.");
            }

            // Yeni şifre hashleme
            string salt = PasswordHelper.CreateSalt();
            string passwordHash = PasswordHelper.HashPassword(request.NewPassword, salt);

            // Şifre güncelleme
            user.PasswordHash = passwordHash;
            user.PasswordSalt = salt;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Şifre sıfırlama talebi başlatma
        /// </summary>
        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                // Güvenlik için kullanıcı bulunamasa bile başarılı yanıt dön
                return;
            }

            // Şifre sıfırlama kodu oluşturma
            int codeLength = _configuration.GetValue<int>("PasswordResetSettings:CodeLength");
            int expirationMinutes = _configuration.GetValue<int>("PasswordResetSettings:ExpirationMinutes");
            string resetToken = PasswordHelper.GenerateResetToken(codeLength);

            // Token kullanıcıya ekleme
            user.RefreshToken = resetToken;
            user.RefreshTokenExpireDate = DateTime.UtcNow.AddMinutes(expirationMinutes);
            await _userRepository.UpdateUserAsync(user);

            // E-posta şablonunu oku ve dinamik değerleri doldur
            string emailTemplate = File.ReadAllText("wwwroot/assets/templates/email/password-reset.html");
            string emailHtml = emailTemplate
                .Replace("{resetCode}", resetToken)
                .Replace("{expirationMinutes}", expirationMinutes.ToString());

            // E-posta gönderme
            await _emailService.SendPasswordResetEmailAsync(user.Email, emailHtml);
        }

        /// <summary>
        /// Şifre sıfırlama işlemi
        /// </summary>
        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null || user.RefreshToken != request.Token || user.RefreshTokenExpireDate <= DateTime.UtcNow)
            {
                throw new Exception("Geçersiz veya süresi dolmuş token.");
            }

            // Yeni şifre hashleme
            string salt = PasswordHelper.CreateSalt();
            string passwordHash = PasswordHelper.HashPassword(request.NewPassword, salt);

            // Şifre güncelleme
            user.PasswordHash = passwordHash;
            user.PasswordSalt = salt;
            user.RefreshToken = null;
            user.RefreshTokenExpireDate = null;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// İki faktörlü kimlik doğrulama ayarlarını güncelleme
        /// </summary>
        public async Task<TwoFactorStatusResponse> SetupTwoFactorAsync(int userId, TwoFactorSetupRequest request)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            // Mevcut şifre doğrulama
            if (!PasswordHelper.VerifyPassword(request.CurrentPassword, user.PasswordSalt, user.PasswordHash))
            {
                throw new Exception("Mevcut şifre hatalı. Güvenlik nedeniyle doğrulama gereklidir.");
            }

            // 2FA global olarak devre dışı ise ve zorunluysa kullanıcıya izin verme
            bool globalEnabled = _twoFactorService.IsTwoFactorEnabled();
            bool globalRequired = _twoFactorService.IsTwoFactorRequired();

            if (!globalEnabled)
            {
                throw new Exception("İki faktörlü kimlik doğrulama şu anda sistem tarafından devre dışı bırakılmıştır.");
            }

            if (globalRequired && !request.Enabled)
            {
                throw new Exception("İki faktörlü kimlik doğrulama sistem tarafından zorunlu kılınmıştır ve devre dışı bırakılamaz.");
            }

            // 2FA ayarlarını güncelle
            user.TwoFactorEnabled = request.Enabled;
            await _userRepository.UpdateUserAsync(user);

            // Yanıt oluştur
            var response = new TwoFactorStatusResponse
            {
                Enabled = user.TwoFactorEnabled,
                IsGloballyRequired = globalRequired,
                Message = user.TwoFactorEnabled
                    ? "İki faktörlü kimlik doğrulama başarıyla etkinleştirildi."
                    : "İki faktörlü kimlik doğrulama devre dışı bırakıldı."
            };

            return response;
        }

        /// <summary>
        /// İki faktörlü kimlik doğrulama durumunu getirme
        /// </summary>
        public async Task<TwoFactorStatusResponse> GetTwoFactorStatusAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            bool globalEnabled = _twoFactorService.IsTwoFactorEnabled();
            bool globalRequired = _twoFactorService.IsTwoFactorRequired();

            return new TwoFactorStatusResponse
            {
                Enabled = user.TwoFactorEnabled,
                IsGloballyRequired = globalRequired,
                Message = !globalEnabled
                    ? "İki faktörlü kimlik doğrulama sistem tarafından devre dışı bırakılmıştır."
                    : globalRequired
                        ? "İki faktörlü kimlik doğrulama sistem tarafından zorunlu kılınmıştır."
                        : user.TwoFactorEnabled
                            ? "İki faktörlü kimlik doğrulama etkin."
                            : "İki faktörlü kimlik doğrulama devre dışı."
            };
        }
    }
}