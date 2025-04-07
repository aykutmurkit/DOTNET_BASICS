using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using TCPListenerLibrary.Core;
using TCPListenerLibrary.Models;

namespace TCPListenerLibrary.Services
{
    /// <summary>
    /// TCP Listener servisi uygulaması
    /// </summary>
    public class TcpListenerService : BackgroundService, ITcpListenerService
    {
        private readonly ILogger<TcpListenerService> _logger;
        private readonly TcpListenerSettings _settings;
        private readonly MessageHandler _messageHandler;
        private TcpListener _tcpListener;
        private bool _isRunning;
        private readonly List<Task> _clientTasks = new();
        private readonly CancellationTokenSource _stoppingCts = new();

        /// <summary>
        /// TcpListenerService constructor'ı
        /// </summary>
        /// <param name="logger">Loglama nesnesi</param>
        /// <param name="options">TCP Listener ayarları</param>
        /// <param name="messageHandler">Mesaj işleme nesnesi</param>
        public TcpListenerService(
            ILogger<TcpListenerService> logger,
            IOptions<TcpListenerSettings> options,
            MessageHandler messageHandler)
        {
            _logger = logger;
            _settings = options.Value;
            _messageHandler = messageHandler;
            _isRunning = false;
        }

        /// <summary>
        /// Servisin çalışıp çalışmadığını döndürür
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Background servis olarak çalıştırma metodu
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartAsync(stoppingToken);
        }

        /// <summary>
        /// TCP sunucusunu başlatır
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_isRunning)
            {
                _logger.LogWarning("TCP sunucusu zaten çalışıyor");
                return;
            }

            try
            {
                IPAddress ipAddress = IPAddress.Parse(_settings.IpAddress);
                _tcpListener = new TcpListener(ipAddress, _settings.Port);
                _tcpListener.Start();
                _isRunning = true;

                _logger.LogInformation("TCP sunucusu {IpAddress}:{Port} adresinde başlatıldı", 
                    _settings.IpAddress, _settings.Port);

                // Bağlantıları dinleme işlemini arka planda çalıştır
                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            _logger.LogDebug("Yeni bağlantı bekleniyor...");
                            
                            // Yeni bir istemci bağlantısı al
                            TcpClient client = await _tcpListener.AcceptTcpClientAsync(cancellationToken);
                            
                            _logger.LogInformation("Yeni istemci bağlandı: {Endpoint}", 
                                client.Client.RemoteEndPoint);
                            
                            // Her istemci için yeni bir task başlat
                            var clientTask = HandleClientAsync(client, _stoppingCts.Token);
                            _clientTasks.Add(clientTask);
                            
                            // Tamamlanan taskları temizle
                            _clientTasks.RemoveAll(t => t.IsCompleted);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("TCP sunucusu dinleme işlemi iptal edildi");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "TCP sunucusu dinleme işlemi sırasında hata oluştu");
                    }
                    finally
                    {
                        _isRunning = false;
                    }
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _isRunning = false;
                _logger.LogError(ex, "TCP sunucusu başlatılırken hata oluştu");
                throw;
            }
        }

        /// <summary>
        /// İstemci bağlantısını işleyen metod
        /// </summary>
        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            using (client)
            {
                try
                {
                    // İstemci için timeout ayarla
                    client.ReceiveTimeout = _settings.ConnectionTimeout;
                    client.SendTimeout = _settings.ConnectionTimeout;
                    
                    var stream = client.GetStream();
                    var buffer = new byte[_settings.BufferSize];
                    
                    while (!cancellationToken.IsCancellationRequested && client.Connected)
                    {
                        // İstemciden veri oku
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        
                        if (bytesRead == 0)
                        {
                            // İstemci bağlantıyı kapatmış
                            _logger.LogInformation("İstemci bağlantıyı kapattı: {Endpoint}", 
                                client.Client.RemoteEndPoint);
                            break;
                        }

                        // Okunan byte'ları işle
                        var receivedBytes = new byte[bytesRead];
                        Array.Copy(buffer, receivedBytes, bytesRead);
                        
                        // Gelen mesajı logla
                        string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedBytes).Trim();
                        _logger.LogInformation("İstemciden mesaj alındı: {Message}", receivedMessage);
                        
                        // Mesajı işle ve yanıt hazırla
                        byte[] responseBytes = _messageHandler.ProcessMessageBytes(receivedBytes);
                        
                        // Yanıtı logla
                        string responseMessage = System.Text.Encoding.UTF8.GetString(responseBytes);
                        _logger.LogInformation("İstemciye gönderilecek yanıt: {Response}", responseMessage);
                        
                        // Yanıtı istemciye gönder
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("İstemci işleme iptal edildi: {Endpoint}", 
                        client.Client.RemoteEndPoint);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "İstemci işleme sırasında hata oluştu: {Endpoint}", 
                        client.Client.RemoteEndPoint);
                }
                finally
                {
                    // Bağlantı kapatılıyor
                    try
                    {
                        client.Close();
                        _logger.LogInformation("İstemci bağlantısı kapatıldı");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "İstemci bağlantısı kapatılırken hata oluştu");
                    }
                }
            }
        }

        /// <summary>
        /// TCP sunucusunu durdurur
        /// </summary>
        Task ITcpListenerService.StopAsync(CancellationToken cancellationToken)
        {
            return StopTcpServerAsync(cancellationToken);
        }

        /// <summary>
        /// TCP sunucusunu durduran yardımcı metod
        /// </summary>
        private async Task StopTcpServerAsync(CancellationToken cancellationToken)
        {
            if (!_isRunning)
            {
                _logger.LogWarning("TCP sunucusu zaten durdurulmuş");
                return;
            }

            try
            {
                _logger.LogInformation("TCP sunucusu durduruluyor...");
                
                // İstemci işlemleri için iptal sinyali gönder
                _stoppingCts.Cancel();
                
                // Tüm istemci taskları tamamlanana kadar bekle
                if (_clientTasks.Count > 0)
                {
                    _logger.LogInformation("Aktif istemci bağlantıları kapatılıyor...");
                    await Task.WhenAll(_clientTasks.ToArray());
                }
                
                // TCP Listener'ı durdur
                _tcpListener.Stop();
                _isRunning = false;
                
                _logger.LogInformation("TCP sunucusu durduruldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TCP sunucusu durdurulurken hata oluştu");
            }
        }

        /// <summary>
        /// Servis durduğunda çağrılan metod
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopTcpServerAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Servis dispose edildiğinde çağrılan metod
        /// </summary>
        public override void Dispose()
        {
            _stoppingCts.Dispose();
            base.Dispose();
        }
    }
} 