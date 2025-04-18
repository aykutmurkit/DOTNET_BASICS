using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Cihaz servis arayüzü
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        /// Tüm cihazları getirir
        /// </summary>
        Task<List<DeviceDto>> GetAllDevicesAsync();

        /// <summary>
        /// ID'ye göre cihaz getirir
        /// </summary>
        Task<DeviceDto> GetDeviceByIdAsync(int id);
        
        /// <summary>
        /// IMEI numarasına göre cihaz getirir
        /// </summary>
        Task<DeviceDto> GetDeviceByImeiAsync(string imei);
        
        /// <summary>
        /// Platform ID'sine göre cihazları getirir
        /// </summary>
        Task<List<DeviceDto>> GetDevicesByPlatformIdAsync(int platformId);
        
        /// <summary>
        /// İstasyon ID'sine göre dolaylı yoldan bağlı tüm cihazları getirir
        /// </summary>
        Task<List<DeviceDto>> GetDevicesByStationIdAsync(int stationId);

        /// <summary>
        /// Cihaz oluşturur
        /// </summary>
        Task<DeviceDto> CreateDeviceAsync(CreateDeviceRequest request);

        /// <summary>
        /// Cihaz günceller
        /// </summary>
        Task<DeviceDto> UpdateDeviceAsync(int id, UpdateDeviceRequest request);

        /// <summary>
        /// Cihaz siler
        /// </summary>
        Task DeleteDeviceAsync(int id);
    }
} 