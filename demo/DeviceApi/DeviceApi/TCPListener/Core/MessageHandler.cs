using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using DeviceApi.TCPListener.Models;
using DeviceApi.TCPListener.Services;
using System.Collections.Concurrent;
using System.Threading;

namespace DeviceApi.TCPListener.Core
{
    /// <summary>
    /// Gelen TCP mesajlarını işleyen ve yanıt üreten sınıf
    /// </summary>
    public class MessageHandler
    {
        private readonly ILogger<MessageHandler> _logger;
        private readonly TcpListenerSettings _settings;
        private readonly IDeviceVerificationService _deviceVerificationService;
        private readonly ConcurrentDictionary<string, (DateTime lastTime, int count)> _recentMessages = new ConcurrentDictionary<string, (DateTime, int)>();
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
        /// <param name="deviceVerificationService">Cihaz doğrulama servisi</param>
        public MessageHandler(
            ILogger<MessageHandler> logger,
            IOptions<TcpListenerSettings> settings,
            IDeviceVerificationService deviceVerificationService)
        {
            _logger = logger;
            _settings = settings.Value;
            _deviceVerificationService = deviceVerificationService;
        }

        /// <summary>
        /// Gelen mesajı işler ve yanıt döndürür
        /// </summary>
        /// <param name="message">İşlenecek mesaj</param>
        /// <returns>İstemciye gönderilecek yanıt</returns>
        public string ProcessMessage(string message)
        {
            // Toplam işlenen mesaj sayacını artır
            Interlocked.Increment(ref _totalProcessedMessages);
            
            // Gelen mesaj için log kısıtlama
            bool shouldLog = ShouldLogMessage(message);
            if (shouldLog)
            {
                _logger.LogInformation("Gelen mesaj işleniyor: {Message}", message);
            }
            else
            {
                // Log kısıtlaması uygulandı
                Interlocked.Increment(ref _throttledLogCount);
            }
            
            try
            {
                // Mesajın formatı: ^TİP+DEĞER1+DEĞER2...~
                if (!IsValidMessage(message))
                {
                    _logger.LogWarning("Geçersiz mesaj formatı: {Message}", message);
                    return CreateErrorResponse("Geçersiz mesaj formatı");
                }
                
                var deviceMessage = ParseMessage(message);
                _logger.LogDebug("İşlenen mesaj: {Message}", JsonSerializer.Serialize(deviceMessage));
                
                var response = ProcessDeviceMessage(deviceMessage);
                var responseMessage = CreateResponse(response);
                
                // Yanıt logunu sadece onaylı cihazlar için bilgi seviyesinde göster
                if (deviceMessage.ApprovalStatus == DeviceApprovalStatus.Approved)
                {
                    _logger.LogInformation("Yanıt hazırlandı: {Response}", responseMessage);
                }
                else
                {
                    // Onaysız cihazlar için debug seviyesinde log tut
                    _logger.LogDebug("Reddedilen cihaz için yanıt: {Response}", responseMessage);
                }
                
                // Handshake mesajları için zamanları izle
                if (deviceMessage.MessageType == MessageType.Handshake)
                {
                    if (response.ResponseCode == ResponseCode.Accept)
                    {
                        _lastSuccessfulHandshake = DateTime.Now;
                    }
                    else if (response.ResponseCode == ResponseCode.Reject)
                    {
                        _lastRejectedHandshake = DateTime.Now;
                    }
                }
                
                return responseMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj işleme sırasında hata oluştu: {Message}", message);
                return CreateErrorResponse("Mesaj işlenemedi");
            }
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
                string message = Encoding.UTF8.GetString(messageBytes).Trim();
                string response = ProcessMessage(message);
                return Encoding.UTF8.GetBytes(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj işleme sırasında hata oluştu");
                return Encoding.UTF8.GetBytes(CreateErrorResponse("Hata: Mesaj işlenemedi"));
            }
        }
        
        /// <summary>
        /// Mesajın formatının geçerli olup olmadığını kontrol eder
        /// </summary>
        private bool IsValidMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;
                
            return message.StartsWith(_settings.StartChar) && 
                   message.EndsWith(_settings.EndChar) &&
                   message.Contains(_settings.DelimiterChar);
        }
        
        /// <summary>
        /// Mesajı ayrıştırıp DeviceMessage nesnesine dönüştürür
        /// </summary>
        private DeviceMessage ParseMessage(string message)
        {
            // Start ve end karakterlerini kaldır
            var content = message.TrimStart(_settings.StartChar).TrimEnd(_settings.EndChar);
            
            // Parametreleri ayır
            var parts = content.Split(_settings.DelimiterChar);
            
            if (parts.Length < 1)
                throw new ArgumentException("Geçersiz mesaj formatı", nameof(message));
                
            // MessageType'ı belirle
            if (!int.TryParse(parts[0], out int messageTypeInt) || !Enum.IsDefined(typeof(MessageType), messageTypeInt))
                throw new ArgumentException($"Geçersiz mesaj tipi: {parts[0]}", nameof(message));
                
            var messageType = (MessageType)messageTypeInt;
            
            var deviceMessage = new DeviceMessage
            {
                MessageType = messageType,
                RawMessage = message,
                ApprovalStatus = DeviceApprovalStatus.Unknown
            };
            
            // Mesaj tipine göre ayrıştırmayı yap
            switch (messageType)
            {
                case MessageType.Handshake:
                    ParseHandshakeMessage(deviceMessage, parts);
                    break;
                    
                // Diğer mesaj tipleri için gerekli parsers eklenecek
                default:
                    _logger.LogWarning("Desteklenmeyen mesaj tipi: {MessageType}", messageType);
                    break;
            }
            
            return deviceMessage;
        }
        
        /// <summary>
        /// El sıkışma mesajını ayrıştırır (Format: ^1+IMEI+ILETISIM_TIPI~)
        /// </summary>
        private void ParseHandshakeMessage(DeviceMessage deviceMessage, string[] parts)
        {
            if (parts.Length < 3)
                throw new ArgumentException("Geçersiz el sıkışma mesajı formatı");
                
            deviceMessage.Imei = parts[1];
            
            if (!int.TryParse(parts[2], out int commTypeInt) || !Enum.IsDefined(typeof(CommunicationType), commTypeInt))
                throw new ArgumentException($"Geçersiz iletişim tipi: {parts[2]}");
                
            deviceMessage.CommunicationType = (CommunicationType)commTypeInt;
            
            // IMEI'ye göre cihazın onaylı olup olmadığını kontrol et
            deviceMessage.ApprovalStatus = _deviceVerificationService.VerifyDeviceByImei(deviceMessage.Imei)
                ? DeviceApprovalStatus.Approved
                : DeviceApprovalStatus.Unapproved;
        }
        
        /// <summary>
        /// Alınan mesaja göre yanıt oluşturur
        /// </summary>
        private DeviceResponse ProcessDeviceMessage(DeviceMessage deviceMessage)
        {
            var response = new DeviceResponse
            {
                MessageType = deviceMessage.MessageType,
                ResponseTime = DateTime.Now.ToString("dd/MM/yy,HH:mm:ss")
            };
            
            switch (deviceMessage.MessageType)
            {
                case MessageType.Handshake:
                    response.ResponseCode = deviceMessage.ApprovalStatus == DeviceApprovalStatus.Approved
                        ? ResponseCode.Accept
                        : ResponseCode.Reject;
                    break;
                    
                // Diğer mesaj tipleri için yanıt oluşturma kodu eklenecek
                
                default:
                    response.ResponseCode = ResponseCode.Reject;
                    break;
            }
            
            // Handshake mesajları için zamanları izle
            if (deviceMessage.MessageType == MessageType.Handshake)
            {
                if (response.ResponseCode == ResponseCode.Accept)
                {
                    _lastSuccessfulHandshake = DateTime.Now;
                }
                else if (response.ResponseCode == ResponseCode.Reject)
                {
                    _lastRejectedHandshake = DateTime.Now;
                }
            }
            
            return response;
        }
        
        /// <summary>
        /// DeviceResponse nesnesinden mesaj formatında yanıt oluşturur
        /// </summary>
        private string CreateResponse(DeviceResponse response)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append(_settings.StartChar);
            sb.Append((int)response.MessageType);
            sb.Append(_settings.DelimiterChar);
            sb.Append((int)response.ResponseCode);
            
            // ResponseCode.Reject ise (2) tarih-saat bilgisi ekleme, direkt sonlandır
            if (response.ResponseCode == ResponseCode.Reject)
            {
                sb.Append(_settings.EndChar);
                return sb.ToString();
            }
            
            // Kabul edilen (ResponseCode.Accept) veya diğer durumlar için tarih-saat ekle
            sb.Append(_settings.DelimiterChar);
            sb.Append(response.ResponseTime);
            sb.Append(_settings.EndChar);
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Hata durumunda yanıt oluşturur
        /// </summary>
        private string CreateErrorResponse(string errorMessage)
        {
            _logger.LogWarning("Hata yanıtı oluşturuluyor: {ErrorMessage}", errorMessage);
            
            return $"{_settings.StartChar}0{_settings.DelimiterChar}2{_settings.DelimiterChar}{DateTime.Now.ToString("dd/MM/yy,HH:mm:ss")}{_settings.EndChar}";
        }

        /// <summary>
        /// Belirli bir mesaj için log yazılıp yazılmayacağını kontrol eder
        /// </summary>
        private bool ShouldLogMessage(string message)
        {
            // IMEI'yi mesajdan çıkarmaya çalış
            string imei = ExtractImeiFromMessage(message);
            if (string.IsNullOrEmpty(imei))
            {
                // IMEI bulunamadı, her zaman logla
                return true;
            }
            
            var now = DateTime.Now;
            var messageKey = imei;
            
            // İlgili IMEI için son log zamanını ve sayısını al veya oluştur
            var (lastLogTime, count) = _recentMessages.GetOrAdd(messageKey, (now, 0));
            
            // Son logdan bu yana geçen süre
            var timeSinceLastLog = now - lastLogTime;
            
            // İlk mesaj veya belirli süre geçtiyse logla
            if (count == 0 || timeSinceLastLog.TotalSeconds >= MESSAGE_LOG_INTERVAL_SECONDS)
            {
                _recentMessages[messageKey] = (now, count + 1);
                return true;
            }
            
            // Sayacı güncelle ama loglama
            _recentMessages[messageKey] = (lastLogTime, count + 1);
            return false;
        }
        
        /// <summary>
        /// Mesaj içinden IMEI değerini çıkarır
        /// </summary>
        private string ExtractImeiFromMessage(string message)
        {
            try
            {
                if (!IsValidMessage(message))
                    return string.Empty;
                    
                // Start ve end karakterlerini kaldır
                var content = message.TrimStart(_settings.StartChar).TrimEnd(_settings.EndChar);
                
                // Parametreleri ayır
                var parts = content.Split(_settings.DelimiterChar);
                
                // Handshake mesajı kontrolü (^1+IMEI+ILETISIM_TIPI~)
                if (parts.Length >= 2 && parts[0] == "1" && !string.IsNullOrEmpty(parts[1]))
                {
                    return parts[1]; // IMEI değeri
                }
                
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
} 