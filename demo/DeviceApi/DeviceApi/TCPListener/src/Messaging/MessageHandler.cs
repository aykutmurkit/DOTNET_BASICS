using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using DeviceApi.TCPListener.Configuration;
using DeviceApi.TCPListener.Protocol;
using DeviceApi.TCPListener.Security;
using System.IO;
using System.Threading.Tasks;

namespace DeviceApi.TCPListener.Messaging
{
    /// <summary>
    /// Gelen TCP mesajlarını işleyen ve yanıt üreten sınıf
    /// </summary>
    public class MessageHandler : IMessageHandler
    {
        private readonly ILogger<MessageHandler> _logger;
        private readonly TcpListenerSettings _settings;
        private readonly IDeviceVerifier _deviceVerifier;
        private readonly ConcurrentDictionary<string, (DateTime lastTime, int count)> _recentMessages = new();
        
        private const int MESSAGE_LOG_INTERVAL_SECONDS = 60; // Aynı mesaj için loglama aralığı
        private long _totalProcessedMessages = 0;
        private long _throttledLogCount = 0;
        private DateTime? _lastSuccessfulHandshake = null;
        private DateTime? _lastRejectedHandshake = null;

        /// <summary>
        /// MessageHandler sınıfının constructor'ı
        /// </summary>
        /// <param name="logger">Loglama için gerekli logger nesnesi</param>
        /// <param name="settings">TCP Listener ayarları</param>
        /// <param name="deviceVerifier">Cihaz doğrulama servisi</param>
        public MessageHandler(
            ILogger<MessageHandler> logger,
            IOptions<TcpListenerSettings> settings,
            IDeviceVerifier deviceVerifier)
        {
            _logger = logger;
            _settings = settings.Value;
            _deviceVerifier = deviceVerifier;
        }

        /// <summary>
        /// İşlenen toplam mesaj sayısını döndürür
        /// </summary>
        public long TotalProcessedMessages => _totalProcessedMessages;
        
        /// <summary>
        /// Log kısıtlaması nedeniyle loglanmayan mesaj sayısını döndürür
        /// </summary>
        public long ThrottledLogCount => _throttledLogCount;
        
        /// <summary>
        /// Son başarılı handshake zamanını döndürür
        /// </summary>
        public DateTime? LastSuccessfulHandshake => _lastSuccessfulHandshake;
        
        /// <summary>
        /// Son reddedilen handshake zamanını döndürür
        /// </summary>
        public DateTime? LastRejectedHandshake => _lastRejectedHandshake;

        /// <summary>
        /// Gelen mesajı işler ve yanıt döndürür
        /// </summary>
        /// <param name="message">İşlenecek mesaj</param>
        /// <returns>İstemciye gönderilecek yanıt</returns>
        public string ProcessMessage(string message)
        {
            _totalProcessedMessages++;
            
            if (string.IsNullOrEmpty(message))
            {
                _logger.LogWarning("Boş mesaj alındı");
                return "ERROR:EMPTY_MESSAGE";
            }
            
            _logger.LogInformation("Mesaj işlendi: {MessageLength} karakter", message.Length);
            
            // Burada mesaj işleme mantığı yer alacak
            // Örnek olarak basit bir yanıt dönüyoruz
            return "OK";
        }

        /// <summary>
        /// Gelen byte mesajını işler ve yanıt döndürür
        /// </summary>
        /// <param name="messageBytes">İşlenecek mesaj byte dizisi</param>
        /// <returns>İstemciye gönderilecek yanıt byte dizisi</returns>
        public byte[] ProcessMessageBytes(byte[] messageBytes)
        {
            _totalProcessedMessages++;
            
            if (messageBytes == null || messageBytes.Length == 0)
            {
                _logger.LogWarning("Boş byte mesajı alındı");
                return Encoding.UTF8.GetBytes("ERROR:EMPTY_MESSAGE");
            }
            
            _logger.LogInformation("Byte mesajı işlendi: {MessageLength} byte", messageBytes.Length);
            
            // Mesajı string'e çevir
            string message = Encoding.UTF8.GetString(messageBytes);
            
            // String mesajı işle
            string response = ProcessMessage(message);
            
            // Yanıtı byte dizisine çevir
            return Encoding.UTF8.GetBytes(response);
        }
        
        /// <summary>
        /// Asenkron olarak mesajı işler
        /// </summary>
        public async Task HandleMessageAsync(byte[] message, Stream stream, string clientAddress)
        {
            try
            {
                _totalProcessedMessages++;
                
                if (message == null || message.Length == 0)
                {
                    _logger.LogWarning("Boş mesaj alındı: {ClientAddress}", clientAddress);
                    byte[] errorResponse = Encoding.UTF8.GetBytes("ERROR:EMPTY_MESSAGE");
                    await stream.WriteAsync(errorResponse, 0, errorResponse.Length);
                    return;
                }
                
                // Mesajı string'e çevir
                string messageString = Encoding.UTF8.GetString(message);
                _logger.LogInformation("Mesaj alındı: {ClientAddress}, Uzunluk: {Length}", 
                    clientAddress, message.Length);
                
                // Cevap hazırla ve gönder
                string response = "OK";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj işlenirken hata oluştu: {ClientAddress}", clientAddress);
            }
        }
        
        /// <summary>
        /// Asenkron olarak string mesajı işler
        /// </summary>
        public async Task HandleMessageAsync(string message, string imei, string clientAddress)
        {
            try
            {
                _totalProcessedMessages++;
                
                if (string.IsNullOrEmpty(message))
                {
                    _logger.LogWarning("Boş mesaj alındı: {ClientAddress}, IMEI: {Imei}", 
                        clientAddress, imei);
                    return;
                }
                
                _logger.LogInformation("Mesaj alındı: {ClientAddress}, IMEI: {Imei}, Uzunluk: {Length}", 
                    clientAddress, imei, message.Length);
                
                // Burada mesaj işleme mantığı yer alacak
                // Örnek olarak basit bir işlem yapıyoruz
                await Task.Delay(10); // Asenkron işlem simülasyonu
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj işlenirken hata oluştu: {ClientAddress}, IMEI: {Imei}", 
                    clientAddress, imei);
            }
        }
    }
} 