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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageType MessageType { get; set; }
        
        /// <summary>
        /// Yanıt kodu
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResponseCode ResponseCode { get; set; }
        
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
    /// Yanıt kodlarını belirten enum
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// Bilinmeyen yanıt kodu
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// Kabul edildi
        /// </summary>
        Accept = 1,
        
        /// <summary>
        /// Reddedildi
        /// </summary>
        Reject = 2
    }
} 