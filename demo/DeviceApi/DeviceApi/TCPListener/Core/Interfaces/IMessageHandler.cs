namespace DeviceApi.TCPListener.Core.Interfaces
{
    /// <summary>
    /// TCP üzerinden gelen mesajları işleyen servis için arayüz
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Gelen string mesajı işler ve yanıt döndürür
        /// </summary>
        /// <param name="message">İşlenecek mesaj</param>
        /// <returns>İstemciye gönderilecek yanıt</returns>
        string ProcessMessage(string message);

        /// <summary>
        /// Gelen byte dizisi mesajı işler ve yanıt döndürür
        /// </summary>
        /// <param name="messageBytes">İşlenecek mesaj byte dizisi</param>
        /// <returns>İstemciye gönderilecek yanıt byte dizisi</returns>
        byte[] ProcessMessageBytes(byte[] messageBytes);
    }
} 