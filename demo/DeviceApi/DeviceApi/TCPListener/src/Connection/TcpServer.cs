using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DeviceApi.TCPListener.Configuration;
using DeviceApi.TCPListener.Connection.Models;
using DeviceApi.TCPListener.Messaging;
using DeviceApi.TCPListener.Security;

namespace DeviceApi.TCPListener.Connection
{
    /// <summary>
    /// TCP sunucu servisi
    /// </summary>
    public class TcpServer : BackgroundService, ITcpServer
    {
        private readonly ILogger<TcpServer> _logger;
        private readonly TcpListenerSettings _settings;
        private readonly IDeviceVerifier _deviceVerifier;
        private readonly Messaging.IMessageHandler _messageHandler;
        
        private TcpListener _listener;
        private bool _isRunning;
        private readonly ConcurrentDictionary<string, TcpClient> _clients = new();
        private readonly object _lockObject = new();
        private DateTime? _startTime;
        
        // İstatistik verileri
        private long _totalConnections;
        private int _connectionsLastMinute;
        private readonly ConcurrentQueue<DateTime> _connectionTimes = new();

        /// <summary>
        /// TCP sunucusunun çalıştığı port numarası
        /// </summary>
        public int Port => _settings.Port;

        /// <summary>
        /// TCP sunucusunun dinlediği IP adresi
        /// </summary>
        public string IpAddress => _settings.ListenAddress;

        /// <summary>
        /// TcpServer constructor
        /// </summary>
        public TcpServer(
            ILogger<TcpServer> logger,
            IOptions<TcpListenerSettings> settings,
            IDeviceVerifier deviceVerifier,
            Messaging.IMessageHandler messageHandler)
        {
            _logger = logger;
            _settings = settings.Value;
            _deviceVerifier = deviceVerifier;
            _messageHandler = messageHandler;
        }

        /// <summary>
        /// TCP sunucusu servisini başlatır
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_isRunning)
            {
                _logger.LogWarning("TCP sunucusu zaten çalışıyor.");
                return;
            }

            try
            {
                IPAddress ipAddress = _settings.ListenAddress == "*" || string.IsNullOrEmpty(_settings.ListenAddress)
                    ? IPAddress.Any
                    : IPAddress.Parse(_settings.ListenAddress);

                _listener = new TcpListener(ipAddress, _settings.Port);
                _listener.Start();
                _isRunning = true;
                _startTime = DateTime.Now;

                _logger.LogInformation("TCP sunucusu başlatıldı. Dinlenen adres: {IpAddress}:{Port}", 
                    ipAddress, _settings.Port);

                // Background service olarak ExecuteAsync metodu otomatik çağrılacak
            }
            catch (Exception ex)
            {
                _isRunning = false;
                _logger.LogError(ex, "TCP sunucusu başlatılırken hata oluştu");
                throw;
            }
        }

        /// <summary>
        /// TCP sunucusu servisini durdurur
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!_isRunning)
            {
                return;
            }

            try
            {
                _isRunning = false;

                // Tüm aktif bağlantıları kapat
                foreach (var client in _clients.Values)
                {
                    client.Close();
                }
                _clients.Clear();

                _listener?.Stop();
                _listener = null;

                _logger.LogInformation("TCP sunucusu durduruldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TCP sunucusu durdurulurken hata oluştu");
            }
        }

        /// <summary>
        /// BackgroundService için ana işlem döngüsü
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TCP sunucu servisi ExecuteAsync başladı");

            while (!stoppingToken.IsCancellationRequested && _isRunning)
            {
                try
                {
                    if (_listener != null && _listener.Pending())
                    {
                        TcpClient client = await _listener.AcceptTcpClientAsync();
                        _totalConnections++;
                        _connectionTimes.Enqueue(DateTime.Now);

                        // Bağlantı limiti kontrolü
                        if (_clients.Count >= _settings.MaxConnections)
                        {
                            _logger.LogWarning("Maksimum bağlantı sayısına ulaşıldı. Yeni bağlantı reddedildi.");
                            client.Close();
                            continue;
                        }

                        // IP adresini al
                        string clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();
                        _clients[clientAddress] = client;

                        _logger.LogInformation("Yeni bağlantı: {ClientAddress}", clientAddress);

                        // Bağlantıyı ayrı bir thread'de işle
                        _ = Task.Run(() => HandleClientAsync(client, clientAddress, stoppingToken), stoppingToken);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Listener kapatıldığında oluşabilir, normal bir durum
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Bağlantı kabul edilirken hata oluştu");
                }

                await Task.Delay(100, stoppingToken); // CPU kullanımını azaltmak için bekleme
            }

            _logger.LogInformation("TCP sunucu servisi ExecuteAsync durduruldu");
        }

        /// <summary>
        /// TCP sunucusu servisinin çalışıp çalışmadığını kontrol eder
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
        /// Aktif bağlantıların adreslerini döndürür
        /// </summary>
        public List<string> GetActiveClientAddresses()
        {
            return new List<string>(_clients.Keys);
        }

        /// <summary>
        /// TCP sunucusu hakkında detaylı istatistik bilgilerini döndürür
        /// </summary>
        public ServerStatistics GetStatistics()
        {
            // Son bir dakikadaki bağlantı sayısını hesapla
            int lastMinuteConnections = 0;
            DateTime oneMinuteAgo = DateTime.Now.AddMinutes(-1);
            
            // Süresi geçmiş bağlantı zamanlarını kuyruktan çıkar
            while (_connectionTimes.TryPeek(out DateTime oldestTime) && oldestTime < oneMinuteAgo)
            {
                _connectionTimes.TryDequeue(out _);
            }
            
            // Kalan kayıtlar, son bir dakikadaki bağlantılardır
            lastMinuteConnections = _connectionTimes.Count;

            string uptime = "00:00:00";
            if (_startTime.HasValue)
            {
                TimeSpan uptimeSpan = DateTime.Now - _startTime.Value;
                uptime = uptimeSpan.ToString(@"hh\:mm\:ss");
            }

            return new ServerStatistics
            {
                IsRunning = _isRunning,
                Port = _settings.Port,
                IpAddress = _settings.ListenAddress,
                ActiveConnections = _clients.Count,
                MaximumConnections = _settings.MaxConnections,
                TotalConnectionsReceived = _totalConnections,
                ActiveThreads = ThreadPool.ThreadCount,
                StartTime = _startTime,
                Uptime = uptime,
                ConnectionsLastMinute = lastMinuteConnections,
                ActiveClientAddresses = GetActiveClientAddresses(),
                RateLimit = new RateLimitStatistics
                {
                    BlacklistedImeiCount = _deviceVerifier.GetBlacklistedDeviceCount(),
                    RateLimitedImeiCount = _deviceVerifier.GetRateLimitedDeviceCount(),
                    BlacklistDurationSeconds = _settings.BlacklistDurationMinutes * 60,
                    RateLimitConfig = $"{_settings.RateLimitPerMinute} request/minute"
                },
                MessageStats = new MessageStatistics
                {
                    TotalProcessedMessages = 0, // Bu değer ayrı bir servisten alınabilir
                    ThrottledLogCount = 0,
                    LastSuccessfulHandshake = null,
                    LastRejectedHandshake = null
                }
            };
        }

        /// <summary>
        /// İstemci bağlantısını işler
        /// </summary>
        private async Task HandleClientAsync(TcpClient client, string clientAddress, CancellationToken cancellationToken)
        {
            try
            {
                using (client)
                {
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[_settings.BufferSize];
                    int bytesRead;

                    while (!cancellationToken.IsCancellationRequested && client.Connected)
                    {
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (bytesRead == 0)
                        {
                            // Bağlantı kapandı
                            break;
                        }

                        // Mesajı işle
                        byte[] message = new byte[bytesRead];
                        Array.Copy(buffer, message, bytesRead);
                        
                        // Mesaj işleme servisine aktar
                        await _messageHandler.HandleMessageAsync(message, stream, clientAddress);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstemci bağlantısı işlenirken hata oluştu: {ClientAddress}", clientAddress);
            }
            finally
            {
                // Bağlantıyı sözlükten kaldır
                _clients.TryRemove(clientAddress, out _);
                _logger.LogInformation("Bağlantı sonlandırıldı: {ClientAddress}", clientAddress);
            }
        }
    }
} 