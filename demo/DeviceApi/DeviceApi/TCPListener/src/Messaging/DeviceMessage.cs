using System.Collections.Generic;

namespace DeviceApi.TCPListener.Messaging
{
    /// <summary>
    /// Cihazdan gelen mesajları temsil eden sınıf
    /// </summary>
    public class DeviceMessage
    {
        /// <summary>
        /// Mesaj tipi
        /// </summary>
        public int MessageType { get; set; }
        
        /// <summary>
        /// Cihazın IMEI numarası
        /// </summary>
        public string Imei { get; set; }
        
        /// <summary>
        /// İletişim tipi
        /// </summary>
        public int CommunicationType { get; set; }
        
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
        
        /// <summary>
        /// Cihaz mesajını oluşturur
        /// </summary>
        public DeviceMessage()
        {
            ApprovalStatus = DeviceApprovalStatus.Unknown;
        }
    }
    
    /// <summary>
    /// Cihaz onay durumunu belirten enum
    /// </summary>
    public enum DeviceApprovalStatus
    {
        /// <summary>
        /// Bilinmeyen durum
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// Cihaz onaylı
        /// </summary>
        Approved = 1,
        
        /// <summary>
        /// Cihaz onaysız
        /// </summary>
        Unapproved = 2
    }
} 