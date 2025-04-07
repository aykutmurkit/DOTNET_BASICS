using Microsoft.Extensions.Configuration;
using System.IO;

namespace TCPListenerLibrary.Extensions
{
    /// <summary>
    /// IConfiguration için eklenti metotlarını içeren sınıf
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// TCP Listener ayarlarını yükler
        /// </summary>
        /// <param name="configuration">Yapılandırma nesnesi</param>
        /// <returns>TCP Listener ayarlarını içeren yapılandırma</returns>
        public static IConfiguration LoadTcpListenerSettings(this IConfiguration configuration)
        {
            // Önce çalışan dizini al
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            
            // Yapılandırma dosyası adı
            string settingsFileName = "TCPListenerSettings.json";
            
            // Yapılandırma dosyasının tam yolu
            string settingsFilePath = Path.Combine(directory, settingsFileName);
            
            // Eğer dosya yoksa ana uygulamanın dizinine göre bir üst dizinde ara
            if (!File.Exists(settingsFilePath))
            {
                string parentDirectory = Directory.GetParent(directory)?.FullName;
                if (parentDirectory != null)
                {
                    settingsFilePath = Path.Combine(parentDirectory, settingsFileName);
                }
            }
            
            // Eğer yine bulunamadıysa, bir üst dizinde daha ara
            if (!File.Exists(settingsFilePath) && Directory.GetParent(directory) != null)
            {
                string parentOfParentDirectory = Directory.GetParent(Directory.GetParent(directory).FullName)?.FullName;
                if (parentOfParentDirectory != null)
                {
                    settingsFilePath = Path.Combine(parentOfParentDirectory, settingsFileName);
                }
            }

            // Dosyayı yükleme ve döndürme
            var builder = new ConfigurationBuilder();
            
            if (File.Exists(settingsFilePath))
            {
                builder.AddJsonFile(settingsFilePath, false, true);
            }
            else
            {
                throw new FileNotFoundException($"TCP Listener yapılandırma dosyası bulunamadı: {settingsFileName}");
            }
            
            return builder.Build();
        }
    }
} 