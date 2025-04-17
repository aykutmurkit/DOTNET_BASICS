namespace DeviceApi.TCPListener.Protocol
{
    /// <summary>
    /// Cihazlara gönderilen yanıt kodlarını tanımlar
    /// </summary>
    public static class ResponseCodes
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
        
        /// <summary>
        /// Yanıt kodunun geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <param name="responseCode">Kontrol edilecek yanıt kodu değeri</param>
        /// <returns>Geçerli ise true, değilse false</returns>
        public static bool IsValid(int responseCode)
        {
            return responseCode >= Unknown && responseCode <= Reject;
        }
        
        /// <summary>
        /// Yanıt kodunun adını döndürür
        /// </summary>
        /// <param name="responseCode">Adı alınacak yanıt kodu değeri</param>
        /// <returns>Yanıt kodunun adı</returns>
        public static string GetName(int responseCode)
        {
            return responseCode switch
            {
                Unknown => "Bilinmeyen",
                Accept => "Kabul Edildi",
                Reject => "Reddedildi",
                _ => "Tanımlanmamış"
            };
        }
    }
} 