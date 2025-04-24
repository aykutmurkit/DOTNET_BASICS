using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using DeviceApi.TCPListener.Core.Interfaces;
using DeviceApi.TCPListener.Models.Configurations;
using DeviceApi.TCPListener.Models.Dto;
using System.Text;
using DeviceApi.TCPListener.Models.Constants;

namespace DeviceApi.TCPListener.Core.Services
{
    /// <summary>
    /// TCP Listener servisinin implementasyonu
    /// </summary>
    public class TcpListenerService : BackgroundService, ITcpListenerService
    {
        private readonly ILogger<TcpListenerService> _logger;
        private readonly IMessageHandler _messageHandler;
        private readonly TcpListenerSettings _settings;
        private readonly IServiceProvider _serviceProvider;

        private TcpListener _listener;
        private bool _isRunning;
        private CancellationTokenSource _stoppingCts;
        private readonly ConcurrentDictionary<string, TcpClient> _clients = new();

        private DateTime? _startTime;
        private long _totalConnectionsReceived = 0;
        private readonly Queue<DateTime> _recentConnections = new Queue<DateTime>();
        private readonly object _connectionLock = new object();

        private int _activeConnectionThreads = 0;

        /// <summary>
        /// TcpListenerService constructor'ı
        /// </summary>
        public TcpListenerService(
            ILogger<TcpListenerService> logger,
            IOptions<TcpListenerSettings> settings,
            IMessageHandler messageHandler,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// TCP Listener'ın çalıştığı port numarası
        /// </summary>
        public int Port => _settings.Port;

        /// <summary>
        /// TCP Listener'ın dinlediği IP adresi
        /// </summary>
        public string IpAddress => _settings.IpAddress;

        /// <summary>
        /// BackgroundService'dan override edilen çalıştırma metodu
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TCP Listener servisi başlatılıyor. IP: {IpAddress}, Port: {Port}",
                _settings.IpAddress, _settings.Port);

            // CancellationTokenSource oluştur
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            try
            {
                // TcpListener'ı oluştur ve başlat
                await StartAsync(_stoppingCts.Token);

                // Token iptal edilene kadar çalışmaya devam et
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex) when (!(ex is OperationCanceledException && stoppingToken.IsCancellationRequested))
            {
                _logger.LogError(ex, "TCP Listener servisinde hata oluştu");
            }
            finally
            {
                // Servis durdurulduğunda listener'ı kapat
                await StopAsync(_stoppingCts.Token);
            }
        }

        /// <summary>
        /// TCP Listener servisini başlatır
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_isRunning)
            {
                _logger.LogWarning("TCP Listener servisi zaten çalışıyor");
                return;
            }

            try
            {
                _startTime = DateTime.Now;

                // IP adresini parse et
                IPAddress ipAddress = IPAddress.Parse(_settings.IpAddress);

                // TcpListener'ı oluştur ve başlat
                _listener = new TcpListener(ipAddress, _settings.Port);
                _listener.Start(_settings.MaxConnections);

                _isRunning = true;
                _logger.LogInformation("TCP Listener servisi başlatıldı. IP: {IpAddress}, Port: {Port}",
                    _settings.IpAddress, _settings.Port);

                // Asenkron olarak istemci bağlantılarını kabul et
                await Task.Factory.StartNew(async () =>
                {
                    await AcceptClientsAsync(cancellationToken);
                }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                _isRunning = false;
                _logger.LogError(ex, "TCP Listener servisi başlatılırken hata oluştu");
                throw;
            }
        }

        /// <summary>
        /// İstemci bağlantılarını kabul eden asenkron metod
        /// </summary>
        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (_isRunning && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // Yeni bir istemci bağlantısı bekleyin
                        TcpClient client = await _listener.AcceptTcpClientAsync();

                        // İstemci socket bilgilerini al
                        var remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                        var clientId = $"{remoteEndPoint.Address}:{remoteEndPoint.Port}";

                        // Bağlantı istatistiklerini güncelle
                        lock (_connectionLock)
                        {
                            _totalConnectionsReceived++;
                            _recentConnections.Enqueue(DateTime.Now);

                            // Son 1 dakikadan eski bağlantıları kuyruktan çıkar
                            while (_recentConnections.Count > 0 &&
                                  (DateTime.Now - _recentConnections.Peek()).TotalMinutes > 1)
                            {
                                _recentConnections.Dequeue();
                            }
                        }

                        // İstemciyi kaydet
                        _clients.TryAdd(clientId, client);

                        _logger.LogInformation("Yeni istemci bağlandı: {ClientId}", clientId);

                        // İstemciyi işlemek için ayrı bir task başlat
                        _ = ProcessClientAsync(client, clientId, cancellationToken);
                    }
                    catch (ObjectDisposedException)
                    {
                        // Listener durdurulduğunda bu istisna normal
                        break;
                    }
                    catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogError(ex, "İstemci bağlantısı kabul edilirken hata oluştu");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // İptal edildiğinde bu istisna normal
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstemci bağlantılarını işlerken hata oluştu");
            }
        }

        /// <summary>
        /// TCP Listener servisini durdurur
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!_isRunning)
            {
                _logger.LogWarning("TCP Listener servisi zaten durdurulmuş");
                return;
            }

            try
            {
                // Tüm istemci bağlantılarını kapat
                await CloseAllClientsAsync();

                // Listener'ı durdur
                _listener?.Stop();
                _isRunning = false;

                _logger.LogInformation("TCP Listener servisi durduruldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TCP Listener servisi durdurulurken hata oluştu");
                throw;
            }
        }

        /// <summary>
        /// TCP Listener servisinin çalışıp çalışmadığını kontrol eder
        /// </summary>
        public bool IsRunning()
        {
            return _isRunning;
        }

        /// <summary>
        /// Bağlı istemci sayısını döndürür
        /// </summary>
        public int GetConnectedClientsCount()
        {
            return _clients.Count;
        }

        /// <summary>
        /// İstemci bağlantısını işler
        /// </summary>
        private async Task ProcessClientAsync(TcpClient client, string clientId, CancellationToken cancellationToken)
        {
            // Thread sayacını artır
            Interlocked.Increment(ref _activeConnectionThreads);

            try
            {
                // Timeout ayarla
                client.ReceiveTimeout = _settings.ConnectionTimeout;
                client.SendTimeout = _settings.ConnectionTimeout;

                using (var networkStream = client.GetStream())
                {
                    byte[] buffer = new byte[_settings.BufferSize];

                    while (client.Connected && !cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            // İstemciden veri oku
                            int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                            if (bytesRead == 0)
                            {
                                // İstemci bağlantıyı kapattı
                                _logger.LogInformation("İstemci bağlantıyı kapattı: {ClientId}", clientId);
                                break;
                            }

                            // Gelen veriyi işle
                            byte[] requestBytes = new byte[bytesRead];
                            Array.Copy(buffer, requestBytes, bytesRead);

                            _logger.LogDebug("İstemciden mesaj alındı: {ClientId}, {BytesRead} byte", clientId, bytesRead);

                            // Mesajı işle ve yanıt oluştur
                            var (responseBytes, deviceInfo) = ProcessClientMessage(requestBytes);

                            // Ana yanıtı gönder
                            await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length, cancellationToken);
                            _logger.LogDebug("İstemciye yanıt gönderildi: {ClientId}, {ResponseLength} byte",
                                clientId, responseBytes.Length);
                            
                            // Eğer cihaz ayarları varsa onları da gönder
                            if (deviceInfo?.DeviceSettings != null)
                            {
                                // Cihaza ayarları göndermeden önce kısa bir bekleme
                                await Task.Delay(200, cancellationToken);
                                
                                byte[] settingsBytes = Encoding.UTF8.GetBytes(deviceInfo.DeviceSettings);
                                await networkStream.WriteAsync(settingsBytes, 0, settingsBytes.Length, cancellationToken);
                                
                                _logger.LogInformation("İstemciye cihaz ayarları gönderildi: {ClientId}, {SettingsLength} byte",
                                    clientId, settingsBytes.Length);
                                    
                                // Eğer ekran mesajları varsa onları da sırayla gönder
                                await SendScreenMessagesAsync(networkStream, deviceInfo, cancellationToken);
                            }
                        }
                        catch (IOException ex)
                        {
                            _logger.LogError(ex, "İstemci ile iletişim sırasında I/O hatası: {ClientId}", clientId);
                            break;
                        }
                        catch (SocketException ex)
                        {
                            _logger.LogError(ex, "İstemci ile iletişim sırasında socket hatası: {ClientId}", clientId);
                            break;
                        }
                        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                        {
                            _logger.LogError(ex, "İstemci mesajı işlenirken hata: {ClientId}", clientId);
                        }
                    }
                }
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "İstemci bağlantısı işlenirken hata: {ClientId}", clientId);
            }
            finally
            {
                // İstemci bağlantısını kapat
                CloseClient(client, clientId);

                // Thread sayacını azalt
                Interlocked.Decrement(ref _activeConnectionThreads);
            }
        }

        /// <summary>
        /// İstemciden gelen mesajı işler ve yanıt oluşturur
        /// </summary>
        private (byte[] responseBytes, DeviceInfoForMessaging deviceInfo) ProcessClientMessage(byte[] requestBytes)
        {
            try
            {
                // MessageHandler ile mesajı işle
                byte[] responseBytes = _messageHandler.ProcessMessageBytes(requestBytes);
                
                // Handshake ve cihaz onayı ile ilgili bilgileri kontrol et
                DeviceInfoForMessaging deviceInfo = ExtractDeviceInfoFromMessage(requestBytes, responseBytes);
                
                return (responseBytes, deviceInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj işleme sırasında hata");
                return (Encoding.UTF8.GetBytes($"{_settings.StartChar}0{_settings.DelimiterChar}0{_settings.DelimiterChar}{DateTime.Now:dd/MM/yy,HH:mm:ss}{_settings.EndChar}"), null);
            }
        }

        /// <summary>
        /// Cihaz mesajları için gerekli bilgileri taşıyan sınıf
        /// </summary>
        private class DeviceInfoForMessaging
        {
            public string Imei { get; set; }
            public string DeviceSettings { get; set; }
            public string FullScreenMessage { get; set; }
            public string ScrollingScreenMessage { get; set; }
            public string BitmapScreenMessage { get; set; }
            public string PredictionMessage { get; set; }
            public bool HasFullScreenMessage { get; set; }
            public bool HasScrollingScreenMessage { get; set; }
            public bool HasBitmapScreenMessage { get; set; }
            public bool HasPredictionMessage { get; set; }
        }

        /// <summary>
        /// İstek ve yanıttan cihaz bilgilerini ve mesajları çıkarır
        /// </summary>
        private DeviceInfoForMessaging ExtractDeviceInfoFromMessage(byte[] requestBytes, byte[] responseBytes)
        {
            try
            {
                string requestStr = Encoding.UTF8.GetString(requestBytes);
                string responseStr = Encoding.UTF8.GetString(responseBytes);
                
                // Handshake mesajı mı kontrol et (^1+...)
                if (requestStr.StartsWith($"{_settings.StartChar}1{_settings.DelimiterChar}"))
                {
                    // Olumlu yanıt mı kontrol et (^1+1+...)
                    if (responseStr.StartsWith($"{_settings.StartChar}1{_settings.DelimiterChar}1{_settings.DelimiterChar}"))
                    {
                        // IMEI al
                        var parts = requestStr.TrimStart(_settings.StartChar).TrimEnd(_settings.EndChar).Split(_settings.DelimiterChar);
                        if (parts.Length >= 2)
                        {
                            string imei = parts[1];
                            _logger.LogInformation("Onaylanan cihaz için mesajlar hazırlanıyor: IMEI {Imei}", imei);
                            
                            // Device ve mesajları al
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var deviceRepository = scope.ServiceProvider.GetRequiredService<Data.Interfaces.IDeviceRepository>();
                                var device = deviceRepository.GetByImei(imei);
                                
                                if (device != null)
                                {
                                    var deviceInfo = new DeviceInfoForMessaging
                                    {
                                        Imei = imei
                                    };
                                    
                                    // Cihaz ayarlarını ekle
                                    if (device.Settings != null)
                                    {
                                        deviceInfo.DeviceSettings = FormatDeviceSettings(device.Settings);
                                    }
                                    
                                    // Ekran mesajlarını ekle
                                    if (device.FullScreenMessage != null)
                                    {
                                        deviceInfo.HasFullScreenMessage = true;
                                        deviceInfo.FullScreenMessage = FormatFullScreenMessage(device.FullScreenMessage);
                                    }
                                    
                                    if (device.ScrollingScreenMessage != null)
                                    {
                                        deviceInfo.HasScrollingScreenMessage = true;
                                        deviceInfo.ScrollingScreenMessage = FormatScrollingScreenMessage(device.ScrollingScreenMessage);
                                    }
                                    
                                    if (device.BitmapScreenMessage != null)
                                    {
                                        deviceInfo.HasBitmapScreenMessage = true;
                                        deviceInfo.BitmapScreenMessage = FormatBitmapScreenMessage(device.BitmapScreenMessage);
                                    }
                                    
                                    // Platformun tahmin verisini ekle
                                    if (device.Platform != null)
                                    {
                                        var predictionRepository = scope.ServiceProvider.GetRequiredService<Data.Interfaces.IPredictionRepository>();
                                        var prediction = predictionRepository.GetByPlatformId(device.PlatformId);
                                        
                                        if (prediction != null)
                                        {
                                            deviceInfo.HasPredictionMessage = true;
                                            deviceInfo.PredictionMessage = FormatPredictionMessage(prediction);
                                            _logger.LogInformation("Cihaz için tahmin verisi hazırlandı: IMEI {Imei}, Platform {PlatformId}", 
                                                imei, device.PlatformId);
                                        }
                                        else
                                        {
                                            _logger.LogWarning("Platform için tahmin verisi bulunamadı: Platform {PlatformId}", device.PlatformId);
                                        }
                                    }
                                    
                                    return deviceInfo;
                                }
                                else
                                {
                                    _logger.LogWarning("IMEI değerine göre cihaz bulunamadı: {Imei}", imei);
                                }
                            }
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz bilgilerini çıkarırken hata oluştu");
                return null;
            }
        }

        /// <summary>
        /// Cihaz ayarlarını istenen formatta düzenler
        /// </summary>
        private string FormatDeviceSettings(Entities.Concrete.DeviceSettings settings)
        {
            StringBuilder formattedSettings = new StringBuilder();
            
            formattedSettings.Append($"{_settings.StartChar}{MessageTypes.DeviceSettings}{_settings.DelimiterChar}");
            
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
        /// Tam ekran mesajını formatlar
        /// </summary>
        private string FormatFullScreenMessage(Entities.Concrete.FullScreenMessage message)
        {
            StringBuilder formattedMessage = new StringBuilder();
            
            // ^6 ile başla ve mesaj tipini belirt
            formattedMessage.Append($"{_settings.StartChar}{MessageTypes.ScreenMessage}{_settings.DelimiterChar}");
            
            // Mesaj tipini belirt (1 = FullScreenMessage)
            formattedMessage.Append("1").Append(_settings.DelimiterChar);
            
            // Türkçe satırlar
            formattedMessage.Append(message.TurkishLine1 ?? string.Empty).Append(_settings.DelimiterChar);
            formattedMessage.Append(message.TurkishLine2 ?? string.Empty).Append(_settings.DelimiterChar);
            formattedMessage.Append(message.TurkishLine3 ?? string.Empty).Append(_settings.DelimiterChar);
            formattedMessage.Append(message.TurkishLine4 ?? string.Empty).Append(_settings.DelimiterChar);
            
            // İngilizce satırlar
            formattedMessage.Append(message.EnglishLine1 ?? string.Empty).Append(_settings.DelimiterChar);
            formattedMessage.Append(message.EnglishLine2 ?? string.Empty).Append(_settings.DelimiterChar);
            formattedMessage.Append(message.EnglishLine3 ?? string.Empty).Append(_settings.DelimiterChar);
            formattedMessage.Append(message.EnglishLine4 ?? string.Empty).Append(_settings.DelimiterChar);
            
            // Font tipi ve hizalama
            formattedMessage.Append(message.FontTypeId).Append(_settings.DelimiterChar);
            formattedMessage.Append(message.AlignmentTypeId);
            
            // Sonu ekle
            formattedMessage.Append(_settings.EndChar);
            
            return formattedMessage.ToString();
        }

        /// <summary>
        /// Kayan ekran mesajını formatlar
        /// </summary>
        private string FormatScrollingScreenMessage(Entities.Concrete.ScrollingScreenMessage message)
        {
            StringBuilder formattedMessage = new StringBuilder();
            
            // ^6 ile başla ve mesaj tipini belirt
            formattedMessage.Append($"{_settings.StartChar}{MessageTypes.ScreenMessage}{_settings.DelimiterChar}");
            
            // Mesaj tipini belirt (2 = ScrollingScreenMessage)
            formattedMessage.Append("2").Append(_settings.DelimiterChar);
            
            // Türkçe ve İngilizce satırlar
            formattedMessage.Append(message.TurkishLine ?? string.Empty).Append(_settings.DelimiterChar);
            formattedMessage.Append(message.EnglishLine ?? string.Empty);
            
            // Sonu ekle
            formattedMessage.Append(_settings.EndChar);
            
            return formattedMessage.ToString();
        }

        /// <summary>
        /// Bitmap ekran mesajını formatlar
        /// </summary>
        private string FormatBitmapScreenMessage(Entities.Concrete.BitmapScreenMessage message)
        {
            StringBuilder formattedMessage = new StringBuilder();
            
            // ^6 ile başla ve mesaj tipini belirt
            formattedMessage.Append($"{_settings.StartChar}{MessageTypes.ScreenMessage}{_settings.DelimiterChar}");
            
            // Mesaj tipini belirt (3 = BitmapScreenMessage)
            formattedMessage.Append("3").Append(_settings.DelimiterChar);
            
            // Türkçe ve İngilizce bitmap
            formattedMessage.Append(message.TurkishBitmap ?? string.Empty).Append(_settings.DelimiterChar);
            formattedMessage.Append(message.EnglishBitmap ?? string.Empty).Append(_settings.DelimiterChar);
            
            // Görüntülenme süresi
            formattedMessage.Append(message.Duration);
            
            // Sonu ekle
            formattedMessage.Append(_settings.EndChar);
            
            return formattedMessage.ToString();
        }

        /// <summary>
        /// Tahmin mesajını formatlar
        /// </summary>
        private string FormatPredictionMessage(Entities.Concrete.Prediction prediction)
        {
            StringBuilder formattedMessage = new StringBuilder();
            
            // ^8 ile başla (Prediction mesaj tipi)
            formattedMessage.Append($"{_settings.StartChar}{MessageTypes.Prediction}{_settings.DelimiterChar}");
            
            // Tahmin bilgilerini ekle
            // Bu kısım platform adı/yönü ve her bir tren bilgisini içerebilir
            formattedMessage.Append("İstasyon: ").Append(prediction.StationName).Append(", ");
            formattedMessage.Append("Yön: ").Append(prediction.Direction).Append(", ");
            
            // İlk tren bilgisi
            if (!string.IsNullOrEmpty(prediction.Train1))
            {
                formattedMessage.Append("Tren 1: ").Append(prediction.Train1);
                
                if (!string.IsNullOrEmpty(prediction.Line1))
                    formattedMessage.Append("/").Append(prediction.Line1);
                    
                if (!string.IsNullOrEmpty(prediction.Destination1))
                    formattedMessage.Append("/").Append(prediction.Destination1);
                    
                if (prediction.Time1.HasValue)
                    formattedMessage.Append("/").Append(prediction.Time1.Value.ToString("HH:mm"));
                
                formattedMessage.Append(", ");
            }
            
            // İkinci tren bilgisi
            if (!string.IsNullOrEmpty(prediction.Train2))
            {
                formattedMessage.Append("Tren 2: ").Append(prediction.Train2);
                
                if (!string.IsNullOrEmpty(prediction.Line2))
                    formattedMessage.Append("/").Append(prediction.Line2);
                    
                if (!string.IsNullOrEmpty(prediction.Destination2))
                    formattedMessage.Append("/").Append(prediction.Destination2);
                    
                if (prediction.Time2.HasValue)
                    formattedMessage.Append("/").Append(prediction.Time2.Value.ToString("HH:mm"));
                
                formattedMessage.Append(", ");
            }
            
            // Üçüncü tren bilgisi
            if (!string.IsNullOrEmpty(prediction.Train3))
            {
                formattedMessage.Append("Tren 3: ").Append(prediction.Train3);
                
                if (!string.IsNullOrEmpty(prediction.Line3))
                    formattedMessage.Append("/").Append(prediction.Line3);
                    
                if (!string.IsNullOrEmpty(prediction.Destination3))
                    formattedMessage.Append("/").Append(prediction.Destination3);
                    
                if (prediction.Time3.HasValue)
                    formattedMessage.Append("/").Append(prediction.Time3.Value.ToString("HH:mm"));
            }
            
            // Sonu ekle
            formattedMessage.Append(_settings.EndChar);
            
            return formattedMessage.ToString();
        }

        /// <summary>
        /// Ekran mesajlarını sırayla gönderir
        /// </summary>
        private async Task SendScreenMessagesAsync(NetworkStream networkStream, DeviceInfoForMessaging deviceInfo, CancellationToken cancellationToken)
        {
            // Atanmış mesajlar varsa gönder
            if (deviceInfo.HasFullScreenMessage)
            {
                await Task.Delay(200, cancellationToken); // Kısa beklemeler ekle
                
                byte[] messageBytes = Encoding.UTF8.GetBytes(deviceInfo.FullScreenMessage);
                await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
                
                _logger.LogInformation("Cihaza FullScreenMessage gönderildi: IMEI {Imei}", deviceInfo.Imei);
            }
            
            if (deviceInfo.HasScrollingScreenMessage)
            {
                await Task.Delay(200, cancellationToken);
                
                byte[] messageBytes = Encoding.UTF8.GetBytes(deviceInfo.ScrollingScreenMessage);
                await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
                
                _logger.LogInformation("Cihaza ScrollingScreenMessage gönderildi: IMEI {Imei}", deviceInfo.Imei);
            }
            
            if (deviceInfo.HasBitmapScreenMessage)
            {
                await Task.Delay(200, cancellationToken);
                
                byte[] messageBytes = Encoding.UTF8.GetBytes(deviceInfo.BitmapScreenMessage);
                await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
                
                _logger.LogInformation("Cihaza BitmapScreenMessage gönderildi: IMEI {Imei}", deviceInfo.Imei);
            }
            
            // Tahmin mesajı göndermesi
            if (deviceInfo.HasPredictionMessage)
            {
                // İlk tahmin mesajını hemen gönder (6'lı mesajlardan sonra)
                await Task.Delay(200, cancellationToken);
                
                byte[] messageBytes = Encoding.UTF8.GetBytes(deviceInfo.PredictionMessage);
                await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
                
                _logger.LogInformation("Cihaza ilk PredictionMessage gönderildi: IMEI {Imei}", deviceInfo.Imei);
                
                // Tahmin mesajlarını 5 saniye aralıklarla göndermeye devam et
                // Bu işlem ayrı bir thread'de çalışacak ve bağlantı kopana kadar devam edecek
                _ = Task.Run(async () => 
                {
                    try
                    {
                        while (!cancellationToken.IsCancellationRequested && networkStream.CanWrite)
                        {
                            // 5 saniye bekle
                            await Task.Delay(5000, cancellationToken);
                            
                            // Tahmin mesajını tekrar gönder
                            byte[] predictionBytes = Encoding.UTF8.GetBytes(deviceInfo.PredictionMessage);
                            await networkStream.WriteAsync(predictionBytes, 0, predictionBytes.Length, cancellationToken);
                            
                            _logger.LogInformation("Cihaza periyodik PredictionMessage gönderildi: IMEI {Imei}", deviceInfo.Imei);
                        }
                    }
                    catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                    {
                        // Bağlantı kapanmış veya hata oluşmuş olabilir
                        _logger.LogWarning(ex, "Periyodik tahmin mesajı gönderimi sırasında hata oluştu: IMEI {Imei}", deviceInfo.Imei);
                    }
                }, cancellationToken);
            }
        }

        /// <summary>
        /// İstemci bağlantısını kapatır
        /// </summary>
        private void CloseClient(TcpClient client, string clientId)
        {
            try
            {
                client.Close();
                _clients.TryRemove(clientId, out _);

                _logger.LogInformation("İstemci bağlantısı kapatıldı: {ClientId}", clientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstemci bağlantısı kapatılırken hata: {ClientId}", clientId);
            }
        }

        /// <summary>
        /// Tüm istemci bağlantılarını kapatır
        /// </summary>
        private async Task CloseAllClientsAsync()
        {
            foreach (var client in _clients)
            {
                CloseClient(client.Value, client.Key);
            }

            _clients.Clear();

            // Biraz bekle
            await Task.Delay(100);

            _logger.LogInformation("Tüm istemci bağlantıları kapatıldı");
        }

        /// <summary>
        /// TCP Listener hakkında detaylı istatistik bilgilerini döndürür
        /// </summary>
        /// <returns>TCP Listener istatistikleri</returns>
        public TcpListenerStatistics GetStatistics()
        {
            // Cihaz doğrulama servisi erişimi için
            var deviceVerificationService = _serviceProvider.GetService<IDeviceVerificationService>();

            var stats = new TcpListenerStatistics
            {
                IsRunning = _isRunning,
                Port = _settings.Port,
                IpAddress = _settings.IpAddress,
                ActiveConnections = _clients.Count,
                MaximumConnections = _settings.MaxConnections,
                TotalConnectionsReceived = _totalConnectionsReceived,
                ActiveThreads = _activeConnectionThreads,
                StartTime = _startTime,
                Uptime = GetUptimeString(),
                ConnectionsLastMinute = GetConnectionsLastMinute(),
                ActiveClientAddresses = GetActiveClientAddresses()
            };

            return stats;
        }

        /// <summary>
        /// Çalışma süresini string olarak döndürür
        /// </summary>
        private string GetUptimeString()
        {
            if (!_startTime.HasValue)
                return "Başlatılmadı";
                
            var uptime = DateTime.Now - _startTime.Value;
            return $"{uptime.Days} gün, {uptime.Hours} saat, {uptime.Minutes} dakika";
        }

        /// <summary>
        /// Son bir dakikadaki bağlantı sayısını döndürür
        /// </summary>
        private int GetConnectionsLastMinute()
        {
            lock (_connectionLock)
            {
                return _recentConnections.Count;
            }
        }

        /// <summary>
        /// Aktif istemci adreslerini döndürür
        /// </summary>
        private List<string> GetActiveClientAddresses()
        {
            return new List<string>(_clients.Keys);
        }
    }
} 