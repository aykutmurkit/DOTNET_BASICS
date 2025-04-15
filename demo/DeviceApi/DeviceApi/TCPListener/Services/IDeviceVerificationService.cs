using System.Collections.Generic;
using DeviceApi.TCPListener.Models.Devices;

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
        /// Onaylı cihazların detaylı bilgilerini döndürür
        /// </summary>
        /// <returns>Onaylı cihazların detaylı bilgileri</returns>
        IEnumerable<ApprovedDeviceDto> GetApprovedDevicesWithDetails();
        
        /// <summary>
        /// Onaysız cihazların listesini döndürür
        /// </summary>
        /// <returns>Onaysız cihazların IMEI listesi</returns>
        IEnumerable<string> GetUnapprovedDevices();
        
        /// <summary>
        /// Onaysız cihazların detaylı bilgilerini döndürür
        /// </summary>
        /// <returns>Onaysız cihazların detaylı bilgileri</returns>
        IEnumerable<UnapprovedDeviceDto> GetUnapprovedDevicesWithDetails();
    }
} 