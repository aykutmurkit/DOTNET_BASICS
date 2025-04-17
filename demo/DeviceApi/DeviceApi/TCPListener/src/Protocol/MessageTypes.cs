namespace DeviceApi.TCPListener.Protocol
{
    /// <summary>
    /// Haberleşme protokolündeki mesaj tiplerini tanımlar
    /// </summary>
    public static class MessageTypes
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
        
        /// <summary>
        /// Mesaj tipinin geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <param name="messageType">Kontrol edilecek mesaj tipi değeri</param>
        /// <returns>Geçerli ise true, değilse false</returns>
        public static bool IsValid(int messageType)
        {
            return messageType >= Unknown && messageType <= Ferry;
        }
        
        /// <summary>
        /// Mesaj tipinin adını döndürür
        /// </summary>
        /// <param name="messageType">Adı alınacak mesaj tipi değeri</param>
        /// <returns>Mesaj tipinin adı</returns>
        public static string GetName(int messageType)
        {
            return messageType switch
            {
                Unknown => "Bilinmeyen",
                Handshake => "Handshake",
                OpeningScreen => "Açılış Ekranı",
                Settings => "Ayarlar",
                Logo => "Logo",
                ClearScreen => "Ekran Temizleme",
                Information => "Bilgilendirme",
                Bus => "Otobüs",
                Tram => "Tramvay",
                Live => "Canlı",
                Ferry => "Vapur",
                _ => "Tanımlanmamış"
            };
        }
    }
} 