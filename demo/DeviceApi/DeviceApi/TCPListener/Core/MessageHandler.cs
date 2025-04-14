using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using DeviceApi.TCPListener.Models;
using DeviceApi.TCPListener.Services;

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
            _logger.LogInformation("Gelen mesaj işleniyor: {Message}", message);
            
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
                
                _logger.LogInformation("Yanıt hazırlandı: {Response}", responseMessage);
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
    }
} 