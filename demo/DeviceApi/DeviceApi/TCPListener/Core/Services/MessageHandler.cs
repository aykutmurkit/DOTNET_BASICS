using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using DeviceApi.TCPListener.Core.Interfaces;
using DeviceApi.TCPListener.Models.Configurations;
using DeviceApi.TCPListener.Models.Entities;
using DeviceApi.TCPListener.Models.Constants;
using Microsoft.Extensions.DependencyInjection;
using Data.Interfaces;
using Entities.Concrete;

namespace DeviceApi.TCPListener.Core.Services
{
    /// <summary>
    /// Gelen TCP mesajlarını işleyen ve yanıt üreten sınıf
    /// </summary>
    public class MessageHandler : IMessageHandler
    {
        private readonly ILogger<MessageHandler> _logger;
        private readonly TcpListenerSettings _settings;
        private readonly IDeviceVerificationService _deviceVerificationService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ConcurrentDictionary<string, (DateTime lastTime, int count)> _recentMessages = new();
        private const int MESSAGE_LOG_INTERVAL_SECONDS = 60; // Aynı mesaj için loglama aralığı
        private long _totalProcessedMessages = 0;
        private long _throttledLogCount = 0;
        private DateTime? _lastSuccessfulHandshake = null;
        private DateTime? _lastRejectedHandshake = null;

        /// <summary>
        /// MessageHandler sınıfının constructor'ı
        /// </summary>
        public MessageHandler(
            ILogger<MessageHandler> logger,
            IOptions<TcpListenerSettings> settings,
            IDeviceVerificationService deviceVerificationService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _deviceVerificationService = deviceVerificationService ?? throw new ArgumentNullException(nameof(deviceVerificationService));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        /// <summary>
        /// Gelen mesajı işler ve yanıt döndürür
        /// </summary>
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
                if (deviceMessage.MessageType == MessageTypes.Handshake)
                {
                    if (response.ResponseCode == ResponseCodes.Accept)
                    {
                        _lastSuccessfulHandshake = DateTime.Now;
                    }
                    else if (response.ResponseCode == ResponseCodes.Reject)
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
            if (!int.TryParse(parts[0], out int messageTypeInt))
                throw new ArgumentException($"Geçersiz mesaj tipi: {parts[0]}", nameof(message));
                
            var deviceMessage = new DeviceMessage
            {
                MessageType = messageTypeInt,
                RawMessage = message,
                ApprovalStatus = DeviceApprovalStatus.Unknown
            };
            
            // Mesaj tipine göre ayrıştırmayı yap
            switch (messageTypeInt)
            {
                case MessageTypes.Handshake:
                    ParseHandshakeMessage(deviceMessage, parts);
                    break;
                    
                // Diğer mesaj tipleri için gerekli parsers eklenecek
                default:
                    _logger.LogWarning("Desteklenmeyen mesaj tipi: {MessageType}", messageTypeInt);
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
            
            if (!int.TryParse(parts[2], out int commTypeInt))
                throw new ArgumentException($"Geçersiz iletişim tipi: {parts[2]}");
                
            deviceMessage.CommunicationType = commTypeInt;
            
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
                case MessageTypes.Handshake:
                    response.ResponseCode = deviceMessage.ApprovalStatus == DeviceApprovalStatus.Approved
                        ? ResponseCodes.Accept
                        : ResponseCodes.Reject;
                    
                    // Eğer handshake kabul edildiyse, cihaz ayarlarını gönder
                    if (response.ResponseCode == ResponseCodes.Accept)
                    {
                        try
                        {
                            var deviceSettings = GetDeviceSettingsByImei(deviceMessage.Imei);
                            if (deviceSettings != null)
                            {
                                // Cihaz ayarlarını formatla ve Response.Data'ya ekle
                                response.Data = FormatDeviceSettings(deviceSettings);
                                _logger.LogInformation("Cihaz ayarları hazırlandı. IMEI: {Imei}, Ayarlar: {Settings}", 
                                    deviceMessage.Imei, response.Data);
                            }
                            else
                            {
                                _logger.LogWarning("Cihaz ayarları bulunamadı. IMEI: {Imei}", deviceMessage.Imei);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Cihaz ayarları hazırlanırken hata oluştu. IMEI: {Imei}", deviceMessage.Imei);
                        }
                    }
                    break;
                    
                // Diğer mesaj tipleri için yanıt oluşturma kodu eklenecek
                
                default:
                    response.ResponseCode = ResponseCodes.Reject;
                    break;
            }
            
            return response;
        }
        
        /// <summary>
        /// IMEI numarasına göre cihaz ayarlarını getirir
        /// </summary>
        private DeviceSettings GetDeviceSettingsByImei(string imei)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                    var device = deviceRepository.GetByImei(imei);
                    
                    if (device != null && device.Settings != null)
                    {
                        return device.Settings;
                    }
                    
                    _logger.LogWarning("IMEI: {Imei} için cihaz veya ayarlar bulunamadı", imei);
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "IMEI: {Imei} için cihaz ayarları getirilirken hata oluştu", imei);
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Cihaz ayarlarını istenen formatta düzenler
        /// </summary>
        /// <param name="settings">Cihaz ayarları</param>
        /// <returns>Formatlanmış ayarlar metni</returns>
        private string FormatDeviceSettings(DeviceSettings settings)
        {
            // Başlangıç formatı: ^3+devicesetting~
            StringBuilder formattedSettings = new StringBuilder();
            
            formattedSettings.Append($"{_settings.StartChar}3{_settings.DelimiterChar}");
            
            // Ayarları ekle
            formattedSettings.Append("ApnName=").Append(settings.ApnName).Append(";");
            formattedSettings.Append("ApnUsername=").Append(settings.ApnUsername ?? string.Empty).Append(";");
            formattedSettings.Append("ApnPassword=").Append(settings.ApnPassword ?? string.Empty).Append(";");
            formattedSettings.Append("ServerIp=").Append(settings.ServerIp).Append(";");
            formattedSettings.Append("TcpPort=").Append(settings.TcpPort).Append(";");
            formattedSettings.Append("UdpPort=").Append(settings.UdpPort).Append(";");
            formattedSettings.Append("FtpStatus=").Append(settings.FtpStatus ? "1" : "0").Append(";");
            
            if (!string.IsNullOrEmpty(settings.FtpIp))
                formattedSettings.Append("FtpIp=").Append(settings.FtpIp).Append(";");
            
            if (settings.FtpPort.HasValue)
                formattedSettings.Append("FtpPort=").Append(settings.FtpPort.Value).Append(";");
            
            if (!string.IsNullOrEmpty(settings.FtpUsername))
                formattedSettings.Append("FtpUsername=").Append(settings.FtpUsername).Append(";");
            
            if (!string.IsNullOrEmpty(settings.FtpPassword))
                formattedSettings.Append("FtpPassword=").Append(settings.FtpPassword).Append(";");
            
            if (settings.ConnectionTimeoutDuration.HasValue)
                formattedSettings.Append("ConnectionTimeoutDuration=").Append(settings.ConnectionTimeoutDuration.Value).Append(";");
            
            if (!string.IsNullOrEmpty(settings.CommunicationHardwareVersion))
                formattedSettings.Append("CommunicationHardwareVersion=").Append(settings.CommunicationHardwareVersion).Append(";");
            
            if (!string.IsNullOrEmpty(settings.CommunicationSoftwareVersion))
                formattedSettings.Append("CommunicationSoftwareVersion=").Append(settings.CommunicationSoftwareVersion).Append(";");
            
            if (!string.IsNullOrEmpty(settings.GraphicsCardHardwareVersion))
                formattedSettings.Append("GraphicsCardHardwareVersion=").Append(settings.GraphicsCardHardwareVersion).Append(";");
            
            if (!string.IsNullOrEmpty(settings.GraphicsCardSoftwareVersion))
                formattedSettings.Append("GraphicsCardSoftwareVersion=").Append(settings.GraphicsCardSoftwareVersion).Append(";");
            
            if (settings.ScrollingTextSpeed.HasValue)
                formattedSettings.Append("ScrollingTextSpeed=").Append(settings.ScrollingTextSpeed.Value).Append(";");
            
            if (!string.IsNullOrEmpty(settings.TramDisplayType))
                formattedSettings.Append("TramDisplayType=").Append(settings.TramDisplayType).Append(";");
            
            if (settings.BusScreenPageCount.HasValue)
                formattedSettings.Append("BusScreenPageCount=").Append(settings.BusScreenPageCount.Value).Append(";");
            
            if (!string.IsNullOrEmpty(settings.TimeDisplayFormat))
                formattedSettings.Append("TimeDisplayFormat=").Append(settings.TimeDisplayFormat).Append(";");
            
            if (!string.IsNullOrEmpty(settings.TramFont))
                formattedSettings.Append("TramFont=").Append(settings.TramFont).Append(";");
            
            if (settings.ScreenVerticalPixelCount.HasValue)
                formattedSettings.Append("ScreenVerticalPixelCount=").Append(settings.ScreenVerticalPixelCount.Value).Append(";");
            
            if (settings.ScreenHorizontalPixelCount.HasValue)
                formattedSettings.Append("ScreenHorizontalPixelCount=").Append(settings.ScreenHorizontalPixelCount.Value).Append(";");
            
            if (settings.TemperatureAlarmThreshold.HasValue)
                formattedSettings.Append("TemperatureAlarmThreshold=").Append(settings.TemperatureAlarmThreshold.Value).Append(";");
            
            if (settings.HumidityAlarmThreshold.HasValue)
                formattedSettings.Append("HumidityAlarmThreshold=").Append(settings.HumidityAlarmThreshold.Value).Append(";");
            
            if (settings.GasAlarmThreshold.HasValue)
                formattedSettings.Append("GasAlarmThreshold=").Append(settings.GasAlarmThreshold.Value).Append(";");
            
            if (settings.LightSensorStatus.HasValue)
                formattedSettings.Append("LightSensorStatus=").Append(settings.LightSensorStatus.Value ? "1" : "0").Append(";");
            
            if (settings.LightSensorOperationLevel.HasValue)
                formattedSettings.Append("LightSensorOperationLevel=").Append(settings.LightSensorOperationLevel.Value).Append(";");
            
            if (settings.LightSensorLevel1.HasValue)
                formattedSettings.Append("LightSensorLevel1=").Append(settings.LightSensorLevel1.Value).Append(";");
            
            if (settings.LightSensorLevel2.HasValue)
                formattedSettings.Append("LightSensorLevel2=").Append(settings.LightSensorLevel2.Value).Append(";");
            
            if (settings.LightSensorLevel3.HasValue)
                formattedSettings.Append("LightSensorLevel3=").Append(settings.LightSensorLevel3.Value).Append(";");
            
            if (!string.IsNullOrEmpty(settings.SocketType))
                formattedSettings.Append("SocketType=").Append(settings.SocketType).Append(";");
            
            if (!string.IsNullOrEmpty(settings.StopName))
                formattedSettings.Append("StopName=").Append(settings.StopName).Append(";");
            
            if (!string.IsNullOrEmpty(settings.StartupLogoFilename))
                formattedSettings.Append("StartupLogoFilename=").Append(settings.StartupLogoFilename).Append(";");
            
            if (!string.IsNullOrEmpty(settings.StartupLogoCrc16))
                formattedSettings.Append("StartupLogoCrc16=").Append(settings.StartupLogoCrc16).Append(";");
            
            if (!string.IsNullOrEmpty(settings.VehicleLogoFilename))
                formattedSettings.Append("VehicleLogoFilename=").Append(settings.VehicleLogoFilename).Append(";");
            
            if (!string.IsNullOrEmpty(settings.VehicleLogoCrc16))
                formattedSettings.Append("VehicleLogoCrc16=").Append(settings.VehicleLogoCrc16).Append(";");
            
            if (!string.IsNullOrEmpty(settings.CommunicationType))
                formattedSettings.Append("CommunicationType=").Append(settings.CommunicationType).Append(";");
            
            // Sonu ekle
            formattedSettings.Append(_settings.EndChar);
            
            return formattedSettings.ToString();
        }
        
        /// <summary>
        /// DeviceResponse nesnesini protokol formatında string'e dönüştürür
        /// </summary>
        private string CreateResponse(DeviceResponse response)
        {
            // Protokol formatı: ^MessageType+ResponseCode+TIME~
            // Örnek: ^1+1+24/12/11,15:46:53~ (handshake kabul)
            // Örnek: ^1+2+24/12/11,15:46:53~ (handshake red)
            
            var responseCode = response.ResponseCode switch
            {
                ResponseCodes.Accept => ResponseCodes.Accept,
                ResponseCodes.Reject => ResponseCodes.Reject,
                _ => ResponseCodes.Error
            };
            
            string baseResponse = $"{_settings.StartChar}{response.MessageType}{_settings.DelimiterChar}" +
                   $"{responseCode}{_settings.DelimiterChar}" +
                   $"{response.ResponseTime}{_settings.EndChar}";
                   
            return baseResponse;
        }
        
        /// <summary>
        /// Hata yanıtı oluşturur
        /// </summary>
        private string CreateErrorResponse(string errorMessage)
        {
            var time = DateTime.Now.ToString("dd/MM/yy,HH:mm:ss");
            _logger.LogWarning("Hata yanıtı oluşturuluyor: {Error}", errorMessage);
            
            // Hata yanıtı: ^0+0+Zaman~ (0=hata mesaj tipi, 0=hata yanıt kodu)
            return $"{_settings.StartChar}0{_settings.DelimiterChar}{ResponseCodes.Error}{_settings.DelimiterChar}{time}{_settings.EndChar}";
        }
        
        /// <summary>
        /// Belirli bir mesaj için log yazılması gerekip gerekmediğini kontrol eder
        /// </summary>
        private bool ShouldLogMessage(string message)
        {
            try
            {
                // Mesajdan IMEI çıkar
                string imei = ExtractImeiFromMessage(message);
                if (string.IsNullOrEmpty(imei))
                    return true; // IMEI çıkarılamıyorsa her zaman logla
                
                string key = $"{imei}:{message.GetHashCode()}";
                
                if (_recentMessages.TryGetValue(key, out var messageInfo))
                {
                    messageInfo.count++;
                    var timeSinceLastLog = DateTime.Now - messageInfo.lastTime;
                    
                    // Belirli bir süre geçmiş veya her 100 mesajda bir logla
                    if (timeSinceLastLog.TotalSeconds >= MESSAGE_LOG_INTERVAL_SECONDS || 
                        messageInfo.count % 100 == 0)
                    {
                        _recentMessages[key] = (DateTime.Now, messageInfo.count);
                        return true;
                    }
                    
                    return false;
                }
                else
                {
                    // İlk kez gelen mesaj
                    _recentMessages[key] = (DateTime.Now, 1);
                    return true;
                }
            }
            catch
            {
                // Hata durumunda güvenli tarafta ol ve mesajı logla
                return true;
            }
        }
        
        /// <summary>
        /// Mesajdan IMEI bilgisini çıkarır
        /// </summary>
        private string ExtractImeiFromMessage(string message)
        {
            try
            {
                // Handshake mesajı ise (^1+IMEI+...)
                if (message.StartsWith($"{_settings.StartChar}1{_settings.DelimiterChar}"))
                {
                    var parts = message.TrimStart(_settings.StartChar)
                                      .TrimEnd(_settings.EndChar)
                                      .Split(_settings.DelimiterChar);
                    
                    if (parts.Length >= 2)
                        return parts[1];
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