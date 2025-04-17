using System;
using System.IO;
using System.Threading.Tasks;

namespace DeviceApi.TCPListener.Messaging
{
    /// <summary>
    /// Mesaj işleme servisi için arayüz
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// İşlenen toplam mesaj sayısını döndürür
        /// </summary>
        long TotalProcessedMessages { get; }
        
        /// <summary>
        /// Log kısıtlaması nedeniyle loglanmayan mesaj sayısını döndürür
        /// </summary>
        long ThrottledLogCount { get; }
        
        /// <summary>
        /// Son başarılı handshake zamanını döndürür
        /// </summary>
        DateTime? LastSuccessfulHandshake { get; }
        
        /// <summary>
        /// Son reddedilen handshake zamanını döndürür
        /// </summary>
        DateTime? LastRejectedHandshake { get; }
        
        /// <summary>
        /// Gelen mesajı işler ve yanıt döndürür
        /// </summary>
        /// <param name="message">İşlenecek mesaj</param>
        /// <returns>İstemciye gönderilecek yanıt</returns>
        string ProcessMessage(string message);
        
        /// <summary>
        /// Gelen byte mesajını işler ve yanıt döndürür
        /// </summary>
        /// <param name="messageBytes">İşlenecek mesaj byte dizisi</param>
        /// <returns>İstemciye gönderilecek yanıt byte dizisi</returns>
        byte[] ProcessMessageBytes(byte[] messageBytes);
        
        /// <summary>
        /// Asenkron olarak mesajı işler
        /// </summary>
        /// <param name="message">İşlenecek mesaj byte dizisi</param>
        /// <param name="stream">Yanıt gönderilebilecek network stream</param>
        /// <param name="clientAddress">İstemci adresi</param>
        /// <returns>Asenkron işlemi</returns>
        Task HandleMessageAsync(byte[] message, Stream stream, string clientAddress);
        
        /// <summary>
        /// Asenkron olarak string mesajı işler
        /// </summary>
        /// <param name="message">İşlenecek mesaj</param>
        /// <param name="imei">Cihaz IMEI</param>
        /// <param name="clientAddress">İstemci adresi</param>
        /// <returns>Asenkron işlemi</returns>
        Task HandleMessageAsync(string message, string imei, string clientAddress);
    }
} 