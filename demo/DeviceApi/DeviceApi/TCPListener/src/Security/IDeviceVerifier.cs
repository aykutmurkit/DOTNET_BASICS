using System.Collections.Generic;
using DeviceApi.TCPListener.Security.Models;

namespace DeviceApi.TCPListener.Security
{
    /// <summary>
    /// Cihaz doğrulama işlemleri için gerekli metotları tanımlar
    /// </summary>
    public interface IDeviceVerifier
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
        /// Onaylı cihazların detaylı bilgilerini döndürür
        /// </summary>
        /// <returns>Onaylı cihazların detaylı bilgileri</returns>
        IEnumerable<ApprovedDeviceInfo> GetApprovedDevicesWithDetails();
        
        /// <summary>
        /// Onaysız cihazların listesini döndürür
        /// </summary>
        /// <returns>Onaysız cihazların IMEI listesi</returns>
        IEnumerable<string> GetUnapprovedDevices();
        
        /// <summary>
        /// Onaysız cihazların detaylı bilgilerini döndürür
        /// </summary>
        /// <returns>Onaysız cihazların detaylı bilgileri</returns>
        IEnumerable<UnapprovedDeviceInfo> GetUnapprovedDevicesWithDetails();
        
        /// <summary>
        /// Kara listedeki cihazların sayısını döndürür
        /// </summary>
        /// <returns>Kara listedeki cihaz sayısı</returns>
        int GetBlacklistedDeviceCount();
        
        /// <summary>
        /// Hız sınırlamasına tabi cihazların sayısını döndürür
        /// </summary>
        /// <returns>Hız sınırlamasına tabi cihaz sayısı</returns>
        int GetRateLimitedDeviceCount();
    }
} 