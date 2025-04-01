using System.Security.Cryptography;
using System.Text;

namespace Core.Security
{
    /// <summary>
    /// Şifre işlemleri yardımcı sınıfı
    /// </summary>
    public class PasswordHelper
    {
        /// <summary>
        /// Şifre için tuz oluşturur
        /// </summary>
        public static string CreateSalt()
        {
            var randomBytes = new byte[32];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        /// <summary>
        /// Şifreyi hashler
        /// </summary>
        public static string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var passwordWithSalt = string.Concat(password, salt);
            var bytes = Encoding.UTF8.GetBytes(passwordWithSalt);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Şifreyi doğrular
        /// </summary>
        public static bool VerifyPassword(string password, string salt, string hash)
        {
            var hashToCheck = HashPassword(password, salt);
            return hash == hashToCheck;
        }

        /// <summary>
        /// Şifre sıfırlama için token oluşturur
        /// </summary>
        public static string GenerateResetToken()
        {
            var randomBytes = new byte[32];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        /// <summary>
        /// Belirli uzunlukta şifre sıfırlama kodu oluşturur
        /// </summary>
        /// <param name="length">Kod uzunluğu</param>
        public static string GenerateResetToken(int length)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);

            // Alfanümerik bir kod oluştur 
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new char[length];
            
            for (int i = 0; i < length; i++)
            {
                // Her byte'ı kullanılabilir karakter kümesine dönüştür
                result[i] = chars[bytes[i] % chars.Length];
            }

            return new string(result);
        }
    }
} 