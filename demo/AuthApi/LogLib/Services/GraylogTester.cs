using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LogLib.Services
{
    /// <summary>
    /// Graylog bağlantısını test etmek için yardımcı sınıf
    /// </summary>
    public class GraylogTester
    {
        private readonly ILogger<GraylogTester> _logger;

        public GraylogTester(ILogger<GraylogTester> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Graylog sunucusuna test bağlantısı yapar
        /// </summary>
        /// <param name="host">Graylog sunucu adresi</param>
        /// <param name="port">Graylog GELF TCP port</param>
        /// <returns>Bağlantı başarılı ise true, değilse false</returns>
        public async Task<bool> TestConnectionAsync(string host, int port)
        {
            _logger.LogInformation($"Graylog bağlantı testi başlatılıyor: {host}:{port}");
            
            try
            {
                using (var client = new TcpClient())
                {
                    client.SendTimeout = 5000;
                    client.ReceiveTimeout = 5000;
                    
                    var connectTask = client.ConnectAsync(host, port);
                    var timeoutTask = Task.Delay(5000); // 5 saniye timeout
                    
                    // Hangisi önce tamamlanırsa
                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                    
                    if (completedTask == timeoutTask)
                    {
                        _logger.LogWarning($"Graylog bağlantısı zaman aşımına uğradı: {host}:{port}");
                        return false;
                    }
                    
                    if (client.Connected)
                    {
                        _logger.LogInformation($"Graylog sunucusuna başarıyla bağlanıldı: {host}:{port}");
                        
                        // Test mesajı gönderme
                        var testMessage = new
                        {
                            version = "1.1",
                            host = Environment.MachineName,
                            short_message = "Graylog test mesajı",
                            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            level = 6, // Info
                            _test = true
                        };
                        
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(testMessage);
                        byte[] messageBytes = Encoding.UTF8.GetBytes(json + '\0');
                        
                        using (NetworkStream stream = client.GetStream())
                        {
                            await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
                            _logger.LogInformation("Test mesajı başarıyla gönderildi");
                        }
                        
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning($"Graylog sunucusuna bağlantı kurulamadı: {host}:{port}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Graylog bağlantı testi sırasında hata oluştu: {ex.Message}");
                return false;
            }
        }
    }
} 