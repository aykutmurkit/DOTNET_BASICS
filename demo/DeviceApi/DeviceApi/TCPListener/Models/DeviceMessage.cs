using System.Text.Json.Serialization;

namespace DeviceApi.TCPListener.Models
{
    /// <summary>
    /// Cihazdan gelen mesajları temsil eden sınıf
    /// </summary>
    public class DeviceMessage
    {
        /// <summary>
        /// Mesaj tipi
        /// </summary>
        public MessageType MessageType { get; set; }
        
        /// <summary>
        /// Cihazın IMEI numarası
        /// </summary>
        public string Imei { get; set; }
        
        /// <summary>
        /// İletişim tipi
        /// </summary>
        public CommunicationType CommunicationType { get; set; }
        
        /// <summary>
        /// Cihazın onay durumu
        /// </summary>
        public DeviceApprovalStatus ApprovalStatus { get; set; }
        
        /// <summary>
        /// Mesajın ham hali
        /// </summary>
        public string RawMessage { get; set; }
        
        /// <summary>
        /// Ek veriler (tipe bağlı olarak değişebilir)
        /// </summary>
        public Dictionary<string, string> AdditionalData { get; set; } = new Dictionary<string, string>();
    }
    
    /// <summary>
    /// Cihaz onay durumunu belirten enum
    /// </summary>
    public enum DeviceApprovalStatus
    {
        /// <summary>
        /// Bilinmeyen durum
        /// </summary>
        Unknown,
        
        /// <summary>
        /// Cihaz onaylı
        /// </summary>
        Approved,
        
        /// <summary>
        /// Cihaz onaysız
        /// </summary>
        Unapproved
    }
} 