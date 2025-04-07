using Microsoft.Extensions.Logging;

namespace TCPListenerLibrary.Core
{
    /// <summary>
    /// Gelen TCP mesajlarını işleyen ve yanıt üreten sınıf
    /// </summary>
    public class MessageHandler
    {
        private readonly ILogger<MessageHandler> _logger;

        /// <summary>
        /// MessageHandler sınıfının constructor'ı
        /// </summary>
        /// <param name="logger">Loglama için gerekli logger nesnesi</param>
        public MessageHandler(ILogger<MessageHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gelen mesajı işler ve yanıt döndürür
        /// </summary>
        /// <param name="message">İşlenecek mesaj</param>
        /// <returns>İstemciye gönderilecek yanıt</returns>
        public string ProcessMessage(string message)
        {
            _logger.LogInformation("Gelen mesaj işleniyor: {Message}", message);

            // Gelen mesajın içeriğine göre farklı yanıtlar oluşturma
            return message switch
            {
                "8" => "8",
                "7" => "7",
                "5" => "67", // 5 için hem 6 hem 7 yanıtı
                _ => $"Alınan mesaj: {message}" // Diğer mesajlar için echo
            };
        }

        /// <summary>
        /// Gelen byte mesajını işler ve yanıt döndürür
        /// </summary>
        /// <param name="messageBytes">İşlenecek mesaj byte dizisi</param>
        /// <returns>İstemciye gönderilecek yanıt byte dizisi</returns>
        public byte[] ProcessMessageBytes(byte[] messageBytes)
        {
            try
            {
                string message = System.Text.Encoding.UTF8.GetString(messageBytes).Trim();
                string response = ProcessMessage(message);
                return System.Text.Encoding.UTF8.GetBytes(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj işleme sırasında hata oluştu");
                return System.Text.Encoding.UTF8.GetBytes("Hata: Mesaj işlenemedi");
            }
        }
    }
} 