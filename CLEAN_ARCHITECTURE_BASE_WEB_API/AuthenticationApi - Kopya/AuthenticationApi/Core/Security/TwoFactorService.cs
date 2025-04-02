using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Core.Security
{
    /// <summary>
    /// İki faktörlü kimlik doğrulama servisi arayüzü.
    /// </summary>
    public interface ITwoFactorService
    {
        /// <summary>
        /// İki faktörlü kimlik doğrulama özelliğinin sistemde aktif olup olmadığını kontrol eder.
        /// </summary>
        bool IsTwoFactorEnabled();

        /// <summary>
        /// İki faktörlü kimlik doğrulamanın tüm kullanıcılar için zorunlu olup olmadığını kontrol eder.
        /// </summary>
        bool IsTwoFactorRequired();

        /// <summary>
        /// Belirli bir kullanıcı için iki faktörlü kimlik doğrulamanın gerekli olup olmadığını belirler.
        /// </summary>
        bool IsTwoFactorRequiredForUser(User user);

        /// <summary>
        /// Yeni bir 2FA kodu oluşturur ve kullanıcıya atar
        /// </summary>
        string GenerateNewCodeForUser(User user);

        /// <summary>
        /// Kullanıcının 2FA kodunu doğrular
        /// </summary>
        bool ValidateCodeForUser(User user, string code);

        /// <summary>
        /// Kullanıcının 2FA kodunu sıfırlar
        /// </summary>
        void ClearTwoFactorCodeForUser(User user);
    }

    /// <summary>
    /// İki faktörlü kimlik doğrulama (2FA) servisi.
    /// Kullanıcı güvenliğini artırmak için ek bir doğrulama katmanı sağlar.
    /// </summary>
    public class TwoFactorService : ITwoFactorService
    {
        private readonly IConfiguration _configuration;

        public TwoFactorService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// İki faktörlü kimlik doğrulama özelliğinin sistemde aktif olup olmadığını kontrol eder.
        /// Bu ayar, 2FA özelliğinin tüm uygulama için açık veya kapalı olmasını belirler.
        /// </summary>
        /// <returns>2FA özelliği sistemde aktifse true, değilse false</returns>
        public bool IsTwoFactorEnabled()
        {
            return _configuration.GetValue<bool>("TwoFactorSettings:SystemEnabled");
        }

        /// <summary>
        /// İki faktörlü kimlik doğrulamanın tüm kullanıcılar için zorunlu olup olmadığını kontrol eder.
        /// Bu ayar true ise, tüm kullanıcılar kendi ayarlarından bağımsız olarak 2FA kullanmak zorundadır.
        /// </summary>
        /// <returns>2FA tüm kullanıcılar için zorunluysa true, değilse false</returns>
        public bool IsTwoFactorRequired()
        {
            return _configuration.GetValue<bool>("TwoFactorSettings:RequiredForAllUsers");
        }

        /// <summary>
        /// Belirli bir kullanıcı için iki faktörlü kimlik doğrulamanın gerekli olup olmadığını belirler.
        /// Karar sırası:
        /// 1. Sistem ayarı kapalıysa - Hiçbir kullanıcı için 2FA gerekmez
        /// 2. Sistem zorunlu kılmışsa - Tüm kullanıcılar için 2FA gerekir
        /// 3. Yukarıdaki durumlar yoksa - Kullanıcının kendi 2FA ayarına bakılır
        /// </summary>
        /// <param name="user">Kontrol edilecek kullanıcı</param>
        /// <returns>Kullanıcı için 2FA gerekiyorsa true, gerekmiyorsa false</returns>
        public bool IsTwoFactorRequiredForUser(User user)
        {
            // Global ayar 2FA'yı devre dışı bırakıyorsa, hiçbir kullanıcı için gerekli değil
            if (!IsTwoFactorEnabled())
                return false;

            // Global ayar zorunlu ise, tüm kullanıcılar için gerekli
            if (IsTwoFactorRequired())
                return true;

            // Aksi takdirde, kullanıcı ayarına bak
            return user.TwoFactorEnabled;
        }

        /// <summary>
        /// Yeni bir 2FA kodu oluşturur ve kullanıcıya atar
        /// </summary>
        public string GenerateNewCodeForUser(User user)
        {
            var codeLength = _configuration.GetValue<int>("TwoFactorSettings:CodeLength");
            var expirationMinutes = _configuration.GetValue<int>("TwoFactorSettings:ExpirationMinutes");

            // Rasgele sayısal kod oluştur
            var code = GenerateRandomNumericCode(codeLength);

            // Kullanıcı bilgilerini güncelle
            user.TwoFactorCode = code;
            user.TwoFactorCodeCreatedAt = DateTime.UtcNow;
            user.TwoFactorCodeExpirationMinutes = expirationMinutes;

            return code;
        }

        /// <summary>
        /// Kullanıcının 2FA kodunu doğrular
        /// </summary>
        /// <returns>Kod doğruysa true, aksi halde false</returns>
        public bool ValidateCodeForUser(User user, string code)
        {
            // Kod boş veya null ise geçersiz
            if (string.IsNullOrEmpty(code))
                return false;

            // Kullanıcıda kayıtlı kod yoksa geçersiz
            if (string.IsNullOrEmpty(user.TwoFactorCode) || !user.TwoFactorCodeCreatedAt.HasValue)
                return false;

            // Kodun süresi dolmuş mu kontrol et
            var expirationTime = user.TwoFactorCodeCreatedAt.Value.AddMinutes(user.TwoFactorCodeExpirationMinutes);
            if (DateTime.UtcNow > expirationTime)
                return false;

            // Kodları karşılaştır
            return user.TwoFactorCode == code;
        }

        /// <summary>
        /// Kullanıcının 2FA kodunu sıfırlar
        /// </summary>
        public void ClearTwoFactorCodeForUser(User user)
        {
            user.TwoFactorCode = null;
            user.TwoFactorCodeCreatedAt = null;
        }

        /// <summary>
        /// Rasgele sayısal kod üretir
        /// </summary>
        private string GenerateRandomNumericCode(int length)
        {
            // Güvenli rastgele sayı üreteci kullan
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);

            // Sayısal bir kod oluştur (her byte 0-9 arasına dönüştürülür)
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                // bytes[i] % 10 işlemi ile 0-9 arasında bir sayı elde edilir
                result[i] = (char)('0' + (bytes[i] % 10));
            }

            return new string(result);
        }
    }
} 