namespace DeviceApi.TCPListener.Models
{
    /// <summary>
    /// Haberleşme protokolündeki mesaj tiplerini tanımlar
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Bilinmeyen mesaj tipi
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// El sıkışma mesajı
        /// </summary>
        Handshake = 1,
        
        /// <summary>
        /// Açılış ekranı mesajı
        /// </summary>
        OpeningScreen = 2,
        
        /// <summary>
        /// Ayarlar mesajı
        /// </summary>
        Settings = 3,
        
        /// <summary>
        /// Logo mesajı
        /// </summary>
        Logo = 4,
        
        /// <summary>
        /// Ekran temizleme mesajı
        /// </summary>
        ClearScreen = 5,
        
        /// <summary>
        /// Bilgilendirme mesajı
        /// </summary>
        Information = 6,
        
        /// <summary>
        /// Otobüs mesajı
        /// </summary>
        Bus = 7,
        
        /// <summary>
        /// Tramvay mesajı
        /// </summary>
        Tram = 8,
        
        /// <summary>
        /// Canlı mesaj
        /// </summary>
        Live = 9,
        
        /// <summary>
        /// Vapur mesajı
        /// </summary>
        Ferry = 10
    }
} 