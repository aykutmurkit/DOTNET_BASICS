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
                            byte[] responseBytes = _messageHandler.ProcessMessageBytes(requestBytes);

                            // Yanıtı gönder
                            await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length, cancellationToken);

                            _logger.LogDebug("İstemciye yanıt gönderildi: {ClientId}, {ResponseLength} byte",
                                clientId, responseBytes.Length);
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