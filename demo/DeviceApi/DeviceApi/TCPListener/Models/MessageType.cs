namespace DeviceApi.TCPListener.Models
{
    /// <summary>
    /// Haberleşme protokolündeki mesaj tiplerini tanımlar
    /// </summary>
    public static class MessageType
    {
        /// <summary>
        /// Bilinmeyen mesaj tipi
        /// </summary>
        public const int Unknown = 0;
        
        /// <summary>
        /// El sıkışma mesajı
        /// </summary>
        public const int Handshake = 1;
        
        /// <summary>
        /// Açılış ekranı mesajı
        /// </summary>
        public const int OpeningScreen = 2;
        
        /// <summary>
        /// Ayarlar mesajı
        /// </summary>
        public const int Settings = 3;
        
        /// <summary>
        /// Logo mesajı
        /// </summary>
        public const int Logo = 4;
        
        /// <summary>
        /// Ekran temizleme mesajı
        /// </summary>
        public const int ClearScreen = 5;
        
        /// <summary>
        /// Bilgilendirme mesajı
        /// </summary>
        public const int Information = 6;
        
        /// <summary>
        /// Otobüs mesajı
        /// </summary>
        public const int Bus = 7;
        
        /// <summary>
        /// Tramvay mesajı
        /// </summary>
        public const int Tram = 8;
        
        /// <summary>
        /// Canlı mesaj
        /// </summary>
        public const int Live = 9;
        
        /// <summary>
        /// Vapur mesajı
        /// </summary>
        public const int Ferry = 10;
    }
} 