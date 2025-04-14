namespace DeviceApi.TCPListener.Services
{
    /// <summary>
    /// Cihaz doğrulama işlemleri için gerekli metotları tanımlar
    /// </summary>
    public interface IDeviceVerificationService
    {
        /// <summary>
        /// Cihazın IMEI numarası ile onaylı olup olmadığını kontrol eder
        /// </summary>
        /// <param name="imei">Kontrol edilecek IMEI numarası</param>
        /// <returns>Cihaz onaylı ise true, değilse false</returns>
        bool VerifyDeviceByImei(string imei);
        
        /// <summary>
        /// Onaylı cihazların listesini döndürür
        /// </summary>
        /// <returns>Onaylı cihazların IMEI listesi</returns>
        IEnumerable<string> GetApprovedDevices();
        
        /// <summary>
        /// Onaysız cihazların listesini döndürür
        /// </summary>
        /// <returns>Onaysız cihazların IMEI listesi</returns>
        IEnumerable<string> GetUnapprovedDevices();
    }
} 