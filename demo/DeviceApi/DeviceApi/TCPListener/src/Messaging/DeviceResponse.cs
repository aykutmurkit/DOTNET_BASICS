using System.Collections.Generic;

namespace DeviceApi.TCPListener.Messaging
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
        
        /// <summary>
        /// Varsayılan yanıt nesnesi oluşturur
        /// </summary>
        public DeviceResponse()
        {
            ResponseCode = Protocol.ResponseCodes.Unknown;
        }
        
        /// <summary>
        /// Belirli bir mesaj tipi ve yanıt koduyla yanıt nesnesi oluşturur
        /// </summary>
        /// <param name="messageType">Yanıt mesaj tipi</param>
        /// <param name="responseCode">Yanıt kodu</param>
        public DeviceResponse(int messageType, int responseCode)
        {
            MessageType = messageType;
            ResponseCode = responseCode;
        }
    }
} 