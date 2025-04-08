using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Cihaz durum servisi arayüzü
    /// </summary>
    public interface IDeviceStatusService
    {
        /// <summary>
        /// Tüm cihaz durumlarını getirir
        /// </summary>
        Task<List<DeviceStatusDto>> GetAllDeviceStatusesAsync();
        
        /// <summary>
        /// ID'ye göre cihaz durumu getirir
        /// </summary>
        Task<DeviceStatusDto> GetDeviceStatusByIdAsync(int id);
        
        /// <summary>
        /// Cihaz ID'sine göre cihaz durumu getirir
        /// </summary>
        Task<DeviceStatusDto> GetDeviceStatusByDeviceIdAsync(int deviceId);
        
        /// <summary>
        /// Yeni cihaz durumu ekler
        /// </summary>
        Task<DeviceStatusDto> AddDeviceStatusAsync(CreateDeviceStatusDto createDeviceStatusDto);
        
        /// <summary>
        /// Cihaz durumunu günceller
        /// </summary>
        Task<DeviceStatusDto> UpdateDeviceStatusAsync(int id, UpdateDeviceStatusDto updateDeviceStatusDto);
        
        /// <summary>
        /// Cihaz durumunu siler
        /// </summary>
        Task DeleteDeviceStatusAsync(int id);
    }
} 