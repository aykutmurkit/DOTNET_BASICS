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
        
        private TcpListener _listener;
        private bool _isRunning;
        private CancellationTokenSource _stoppingCts;
        private readonly ConcurrentDictionary<string, TcpClient> _clients = new();
        
        /// <summary>
        /// TcpListenerService constructor'ı
        /// </summary>
        public TcpListenerService(
            ILogger<TcpListenerService> logger,
            IOptions<TcpListenerSettings> settings,
            MessageHandler messageHandler)
        {
            _logger = logger;
            _settings = settings.Value;
            _messageHandler = messageHandler;
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
    }
} 