using System.Text.Json.Serialization;

namespace DeviceApi.TCPListener.Models
{
    /// <summary>
    /// Cihazlara gönderilecek yanıtları temsil eden sınıf
    /// </summary>
    public class DeviceResponse
    {
        /// <summary>
        /// Yanıt mesajının tipi
        /// </summary>
        public int MessageType { get; set; }
        
        /// <summary>
        /// Yanıt kodu
        /// </summary>
        public int ResponseCode { get; set; }
        
        /// <summary>
        /// Yanıt zamanı (dd/MM/yy,HH:mm:ss formatında)
        /// </summary>
        public string ResponseTime { get; set; }
        
        /// <summary>
        /// Ek veri (yanıt tipine bağlı olarak değişebilir)
        /// </summary>
        public Dictionary<string, string> AdditionalData { get; set; } = new Dictionary<string, string>();
    }
    
    /// <summary>
    /// Yanıt kodlarını belirten sınıf
    /// </summary>
    public static class ResponseCode
    {
        /// <summary>
        /// Bilinmeyen yanıt kodu
        /// </summary>
        public const int Unknown = 0;
        
        /// <summary>
        /// Kabul edildi
        /// </summary>
        public const int Accept = 1;
        
        /// <summary>
        /// Reddedildi
        /// </summary>
        public const int Reject = 2;
    }
} 