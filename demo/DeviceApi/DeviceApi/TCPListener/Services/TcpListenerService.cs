using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DeviceApi.TCPListener.Core;
using DeviceApi.TCPListener.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceApi.TCPListener.Services
{
    /// <summary>
    /// TCP Listener servisinin implementasyonu
    /// </summary>
    public class TcpListenerService : BackgroundService, ITcpListenerService
    {
        private readonly ILogger<TcpListenerService> _logger;
        private readonly MessageHandler _messageHandler;
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
        private readonly object _threadCountLock = new object();
        
        /// <summary>
        /// TcpListenerService constructor'ı
        /// </summary>
        public TcpListenerService(
            ILogger<TcpListenerService> logger,
            IOptions<TcpListenerSettings> settings,
            MessageHandler messageHandler,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _settings = settings.Value;
            _messageHandler = messageHandler;
            _serviceProvider = serviceProvider;
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
            // Cihaz doğrulama servisi ve MessageHandler'a erişim için
            var deviceVerificationService = _serviceProvider.GetService<IDeviceVerificationService>();
            var messageHandler = _serviceProvider.GetService<MessageHandler>();
            
            // Kara liste ve rate limit istatistikleri
            var rateLimitStats = new RateLimitStatistics
            {
                BlacklistedImeiCount = GetBlacklistedImeiCount(deviceVerificationService),
                RateLimitedImeiCount = GetRateLimitedImeiCount(deviceVerificationService),
                BlacklistDurationSeconds = GetBlacklistDuration(deviceVerificationService),
                RateLimitConfig = GetRateLimitConfig(deviceVerificationService)
            };
            
            // Mesaj işleme istatistikleri
            var messageStats = new MessageStatistics
            {
                TotalProcessedMessages = GetTotalProcessedMessages(messageHandler),
                ThrottledLogCount = GetThrottledLogCount(messageHandler),
                LastSuccessfulHandshake = GetLastSuccessfulHandshake(messageHandler),
                LastRejectedHandshake = GetLastRejectedHandshake(messageHandler)
            };
            
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
                ActiveClientAddresses = GetActiveClientAddresses(),
                RateLimit = rateLimitStats,
                MessageStats = messageStats
            };
            
            return stats;
        }
        
        /// <summary>
        /// Kara listedeki IMEI sayısını alır
        /// </summary>
        private int GetBlacklistedImeiCount(IDeviceVerificationService deviceVerificationService)
        {
            // Burada reflection veya sadece yaklaşık bir değer kullanabilirsiniz
            // Rate limit bilgileri genellikle private olduğundan, bu bir tahmin olabilir
            try 
            {
                var type = deviceVerificationService.GetType();
                var field = type.GetField("_blacklistedImeis", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null && field.GetValue(deviceVerificationService) is System.Collections.IDictionary dict)
                {
                    return dict.Count;
                }
            }
            catch 
            {
                // Sessizce hatayı yut
            }
            return -1; // Bilinmiyor
        }
        
        /// <summary>
        /// Rate limit takip edilen IMEI sayısını alır
        /// </summary>
        private int GetRateLimitedImeiCount(IDeviceVerificationService deviceVerificationService)
        {
            try 
            {
                var type = deviceVerificationService.GetType();
                var field = type.GetField("_rateLimitTracker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null && field.GetValue(deviceVerificationService) is System.Collections.IDictionary dict)
                {
                    return dict.Count;
                }
            }
            catch 
            {
                // Sessizce hatayı yut
            }
            return -1; // Bilinmiyor
        }
        
        /// <summary>
        /// Kara liste süresini alır
        /// </summary>
        private int GetBlacklistDuration(IDeviceVerificationService deviceVerificationService)
        {
            try 
            {
                var type = deviceVerificationService.GetType();
                var field = type.GetField("BLACKLIST_DURATION_SECONDS", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (field != null)
                {
                    return (int)field.GetValue(null);
                }
            }
            catch 
            {
                // Sessizce hatayı yut
            }
            return 300; // Varsayılan değer
        }
        
        /// <summary>
        /// Rate limit konfigürasyonunu alır
        /// </summary>
        private string GetRateLimitConfig(IDeviceVerificationService deviceVerificationService)
        {
            try 
            {
                var type = deviceVerificationService.GetType();
                var maxField = type.GetField("RATE_LIMIT_MAX_REQUESTS", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var windowField = type.GetField("RATE_LIMIT_WINDOW_SECONDS", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                
                if (maxField != null && windowField != null)
                {
                    int max = (int)maxField.GetValue(null);
                    int window = (int)windowField.GetValue(null);
                    return $"{max} istek / {window} saniye";
                }
            }
            catch 
            {
                // Sessizce hatayı yut
            }
            return "5 istek / 1 saniye"; // Varsayılan değer
        }
        
        /// <summary>
        /// Toplam işlenen mesaj sayısını alır
        /// </summary>
        private long GetTotalProcessedMessages(MessageHandler messageHandler)
        {
            if (messageHandler == null) return 0;
            
            try 
            {
                var type = messageHandler.GetType();
                var field = type.GetField("_totalProcessedMessages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    return (long)field.GetValue(messageHandler);
                }
            }
            catch 
            {
                // Sessizce hatayı yut
            }
            return -1; // Bilinmiyor
        }
        
        /// <summary>
        /// Kısıtlanan log sayısını alır
        /// </summary>
        private long GetThrottledLogCount(MessageHandler messageHandler)
        {
            if (messageHandler == null) return 0;
            
            try 
            {
                var type = messageHandler.GetType();
                var field = type.GetField("_throttledLogCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    return (long)field.GetValue(messageHandler);
                }
            }
            catch 
            {
                // Sessizce hatayı yut
            }
            return -1; // Bilinmiyor
        }
        
        /// <summary>
        /// Son başarılı handshake zamanını alır
        /// </summary>
        private DateTime? GetLastSuccessfulHandshake(MessageHandler messageHandler)
        {
            if (messageHandler == null) return null;
            
            try 
            {
                var type = messageHandler.GetType();
                var field = type.GetField("_lastSuccessfulHandshake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    return (DateTime?)field.GetValue(messageHandler);
                }
            }
            catch 
            {
                // Sessizce hatayı yut
            }
            return null;
        }
        
        /// <summary>
        /// Son reddedilen handshake zamanını alır
        /// </summary>
        private DateTime? GetLastRejectedHandshake(MessageHandler messageHandler)
        {
            if (messageHandler == null) return null;
            
            try 
            {
                var type = messageHandler.GetType();
                var field = type.GetField("_lastRejectedHandshake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    return (DateTime?)field.GetValue(messageHandler);
                }
            }
            catch 
            {
                // Sessizce hatayı yut
            }
            return null;
        }
        
        /// <summary>
        /// Aktif bağlantı thread sayısını döndürür
        /// </summary>
        private int GetActiveThreadCount()
        {
            return _activeConnectionThreads;
        }
        
        /// <summary>
        /// Çalışma süresini hesaplar
        /// </summary>
        private string GetUptimeString()
        {
            if (!_startTime.HasValue)
                return "00:00:00";
                
            var uptime = DateTime.Now - _startTime.Value;
            return $"{uptime.Hours:D2}:{uptime.Minutes:D2}:{uptime.Seconds:D2}";
        }
        
        /// <summary>
        /// Son dakika içinde alınan bağlantı sayısını hesaplar
        /// </summary>
        private int GetConnectionsLastMinute()
        {
            lock (_connectionLock)
            {
                return _recentConnections.Count;
            }
        }
        
        /// <summary>
        /// Aktif bağlı cihazların adreslerini döndürür
        /// </summary>
        private List<string> GetActiveClientAddresses()
        {
            return _clients.Keys.ToList();
        }
    }
} 