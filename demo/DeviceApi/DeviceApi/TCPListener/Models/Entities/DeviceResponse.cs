using DeviceApi.TCPListener.Models.Constants;

namespace DeviceApi.TCPListener.Models.Entities
{
    /// <summary>
    /// Cihaza gönderilecek yanıt bilgilerini tutan sınıf
    /// </summary>
    public class DeviceResponse
    {
        /// <summary>
        /// Yanıt kodu
        /// </summary>
        public int ResponseCode { get; set; }

        /// <summary>
        /// İşlenen mesajın tipi
        /// </summary>
        public int MessageType { get; set; }

        /// <summary>
        /// Yanıt zamanı
        /// </summary>
        public string ResponseTime { get; set; }

        /// <summary>
        /// Yanıt verisi
        /// </summary>
        public string Data { get; set; }
    }
} 