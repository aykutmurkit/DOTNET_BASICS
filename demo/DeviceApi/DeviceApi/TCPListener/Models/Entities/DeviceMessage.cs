using DeviceApi.TCPListener.Models.Constants;

namespace DeviceApi.TCPListener.Models.Entities
{
    /// <summary>
    /// Cihazdan gelen mesaj bilgilerini tutan sınıf
    /// </summary>
    public class DeviceMessage
    {
        /// <summary>
        /// Mesaj tipi
        /// </summary>
        public int MessageType { get; set; }

        /// <summary>
        /// Mesajı gönderen cihazın IMEI numarası
        /// </summary>
        public string Imei { get; set; }

        /// <summary>
        /// İletişim tipi
        /// </summary>
        public int CommunicationType { get; set; }

        /// <summary>
        /// Cihazın onay durumu
        /// </summary>
        public int ApprovalStatus { get; set; }

        /// <summary>
        /// Ham mesaj verisi
        /// </summary>
        public string RawMessage { get; set; }

        /// <summary>
        /// Mesajın alındığı IP adresi
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Varsa, mesajdan çıkarılan ek veriler
        /// </summary>
        public string Data { get; set; }
    }
} 