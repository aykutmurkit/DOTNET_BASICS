using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Core.Utilities
{
    /// <summary>
    /// E-posta gönderme servisi
    /// </summary>
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Şifre sıfırlama e-postası gönderir
        /// </summary>
        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var subject = "Şifre Sıfırlama";
            var body = $@"
                <h1>Şifre Sıfırlama</h1>
                <p>Şifrenizi sıfırlamak için aşağıdaki kodu kullanın:</p>
                <h2>{resetToken}</h2>
                <p>Bu kod 24 saat süreyle geçerlidir.</p>
            ";

            await SendEmailAsync(email, subject, body);
        }

        /// <summary>
        /// İki faktörlü kimlik doğrulama kodu e-postası gönderir
        /// </summary>
        public async Task SendTwoFactorCodeEmailAsync(string email, string username, string code, int expirationMinutes)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var subject = "Güvenlik Kodu - İki Faktörlü Kimlik Doğrulama";
            
            var body = GetTwoFactorEmailTemplate(username, code, expirationMinutes);

            await SendEmailAsync(email, subject, body);
        }

        /// <summary>
        /// İki faktörlü kimlik doğrulama e-posta şablonu
        /// </summary>
        private string GetTwoFactorEmailTemplate(string username, string code, int expirationMinutes)
        {
            // Formatlanmış kodun CSS-ile güzel gösterimi için
            var formattedCode = string.Join(" ", code.ToCharArray());
            
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>İki Faktörlü Kimlik Doğrulama</title>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        background-color: #f9f9f9;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        background-color: #ffffff;
                        border-radius: 8px;
                        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                    }}
                    .header {{
                        text-align: center;
                        padding: 20px 0;
                        border-bottom: 1px solid #eaeaea;
                    }}
                    .header h1 {{
                        color: #2c3e50;
                        margin: 0;
                        font-size: 24px;
                    }}
                    .content {{
                        padding: 30px 20px;
                        text-align: center;
                    }}
                    .code-container {{
                        margin: 30px 0;
                        padding: 20px;
                        background-color: #f5f7f9;
                        border-radius: 6px;
                        font-family: monospace;
                        font-size: 32px;
                        font-weight: bold;
                        letter-spacing: 8px;
                        color: #2c3e50;
                    }}
                    .footer {{
                        text-align: center;
                        padding: 20px 0;
                        color: #7f8c8d;
                        font-size: 14px;
                        border-top: 1px solid #eaeaea;
                    }}
                    .button {{
                        display: inline-block;
                        padding: 12px 24px;
                        background-color: #3498db;
                        color: white;
                        text-decoration: none;
                        border-radius: 4px;
                        font-weight: bold;
                        margin-top: 20px;
                    }}
                    .warning {{
                        color: #e74c3c;
                        margin-top: 20px;
                        font-weight: bold;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Güvenlik Kodu</h1>
                    </div>
                    <div class='content'>
                        <p>Merhaba <strong>{username}</strong>,</p>
                        <p>Hesabınıza giriş yapmak için gereken güvenlik kodunuz aşağıdadır:</p>
                        
                        <div class='code-container'>
                            {formattedCode}
                        </div>
                        
                        <p>Bu kod <strong>{expirationMinutes} dakika</strong> boyunca geçerlidir.</p>
                        
                        <p class='warning'>Bu kodu kimseyle paylaşmayın!</p>
                        <p>Eğer bu giriş AuthenticationApisini siz yapmadıysanız, lütfen şifrenizi değiştirin.</p>
                    </div>
                    <div class='footer'>
                        <p>Bu e-posta, hesap güvenliğiniz için otomatik olarak gönderilmiştir.</p>
                        <p>&copy; {DateTime.Now.Year} - AuthenticationApi API</p>
                    </div>
                </div>
            </body>
            </html>";
        }

        /// <summary>
        /// Yeni kullanıcıya otomatik oluşturulan şifre bilgisini gönderir
        /// </summary>
        public async Task SendRandomPasswordEmailAsync(string email, string username, string password)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var subject = "Hoş Geldiniz - Yeni Hesap Bilgileriniz";
            
            var body = GetRandomPasswordEmailTemplate(username, password);

            await SendEmailAsync(email, subject, body);
        }

        /// <summary>
        /// Otomatik oluşturulan şifre e-posta şablonu
        /// </summary>
        private string GetRandomPasswordEmailTemplate(string username, string password)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Hoş Geldiniz - Yeni Hesap Bilgileriniz</title>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        background-color: #f9f9f9;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        background-color: #ffffff;
                        border-radius: 8px;
                        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                    }}
                    .header {{
                        text-align: center;
                        padding: 20px 0;
                        border-bottom: 1px solid #eaeaea;
                    }}
                    .header h1 {{
                        color: #2c3e50;
                        margin: 0;
                        font-size: 24px;
                    }}
                    .content {{
                        padding: 30px 20px;
                        text-align: center;
                    }}
                    .password-container {{
                        margin: 30px 0;
                        padding: 20px;
                        background-color: #f5f7f9;
                        border-radius: 6px;
                        font-family: monospace;
                        font-size: 24px;
                        font-weight: bold;
                        letter-spacing: 2px;
                        color: #2c3e50;
                    }}
                    .footer {{
                        text-align: center;
                        padding: 20px 0;
                        color: #7f8c8d;
                        font-size: 14px;
                        border-top: 1px solid #eaeaea;
                    }}
                    .button {{
                        display: inline-block;
                        padding: 12px 24px;
                        background-color: #3498db;
                        color: white;
                        text-decoration: none;
                        border-radius: 4px;
                        font-weight: bold;
                        margin-top: 20px;
                    }}
                    .warning {{
                        color: #e74c3c;
                        margin-top: 20px;
                        font-weight: bold;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Hoş Geldiniz!</h1>
                    </div>
                    <div class='content'>
                        <p>Merhaba <strong>{username}</strong>,</p>
                        <p>AuthenticationApi API sistemine hoş geldiniz. Hesabınız başarıyla oluşturuldu.</p>
                        
                        <p>Hesabınıza giriş yapmak için otomatik oluşturulan şifreniz:</p>
                        
                        <div class='password-container'>
                            {password}
                        </div>
                        
                        <p class='warning'>Bu şifre size sistem tarafından atanmıştır ve şu an sadece siz bilmektesiniz.</p>
                        <p>Güvenliğiniz için en kısa sürede şifrenizi değiştirmenizi öneririz.</p>
                        
                        <a href='{_configuration["AppSettings:WebAppUrl"]}/login' class='button'>Giriş Yap</a>
                    </div>
                    <div class='footer'>
                        <p>Bu e-posta, hesap güvenliğiniz için otomatik olarak gönderilmiştir.</p>
                        <p>&copy; {DateTime.Now.Year} - AuthenticationApi API</p>
                    </div>
                </div>
            </body>
            </html>";
        }

        /// <summary>
        /// Genel e-posta gönderme metodu
        /// </summary>
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            
            using var client = new SmtpClient(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]))
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["Password"])
            };

            using var message = new MailMessage
            {
                From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            await client.SendMailAsync(message);
        }
    }
} 