using Entities.Dtos;

namespace AuthApi.Business.Services.Interfaces
{
    /// <summary>
    /// Kimlik doğrulama servis arayüzü
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Kullanıcı kayıt işlemi
        /// </summary>
        Task<AuthResponse> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Kullanıcı giriş işlemi
        /// </summary>
        Task<object> LoginAsync(LoginRequest request);

        /// <summary>
        /// İki faktörlü kimlik doğrulama aşaması
        /// </summary>
        Task<AuthResponse> VerifyTwoFactorAsync(TwoFactorVerifyRequest request);

        /// <summary>
        /// Token yenileme işlemi
        /// </summary>
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);

        /// <summary>
        /// Şifre değiştirme işlemi
        /// </summary>
        Task ChangePasswordAsync(int userId, ChangePasswordRequest request);

        /// <summary>
        /// Şifre sıfırlama talebi başlatma
        /// </summary>
        Task ForgotPasswordAsync(ForgotPasswordRequest request);

        /// <summary>
        /// Şifre sıfırlama işlemi
        /// </summary>
        Task ResetPasswordAsync(ResetPasswordRequest request);

        /// <summary>
        /// İki faktörlü kimlik doğrulama ayarlarını güncelleme
        /// </summary>
        Task<TwoFactorStatusResponse> SetupTwoFactorAsync(int userId, TwoFactorSetupRequest request);

        /// <summary>
        /// İki faktörlü kimlik doğrulama durumunu getirme
        /// </summary>
        Task<TwoFactorStatusResponse> GetTwoFactorStatusAsync(int userId);
    }
}