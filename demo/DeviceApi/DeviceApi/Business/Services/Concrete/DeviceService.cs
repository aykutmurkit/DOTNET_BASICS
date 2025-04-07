using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Cihaz işlemlerini yöneten servis sınıfı
    /// </summary>
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IPlatformRepository _platformRepository;
        private readonly IStationRepository _stationRepository;
        private readonly IDeviceSettingsRepository _deviceSettingsRepository;

        /// <summary>
        /// DeviceService sınıfının constructor'ı
        /// </summary>
        /// <param name="deviceRepository">Cihaz veritabanı işlemleri için repository</param>
        /// <param name="platformRepository">Platform veritabanı işlemleri için repository</param>
        /// <param name="stationRepository">İstasyon veritabanı işlemleri için repository</param>
        /// <param name="deviceSettingsRepository">Cihaz ayarları veritabanı işlemleri için repository</param>
        public DeviceService(
            IDeviceRepository deviceRepository, 
            IPlatformRepository platformRepository, 
            IStationRepository stationRepository,
            IDeviceSettingsRepository deviceSettingsRepository)
        {
            _deviceRepository = deviceRepository;
            _platformRepository = platformRepository;
            _stationRepository = stationRepository;
            _deviceSettingsRepository = deviceSettingsRepository;
        }

        /// <summary>
        /// Tüm cihazları getirir
        /// </summary>
        /// <returns>Cihaz listesi</returns>
        public async Task<List<DeviceDto>> GetAllDevicesAsync()
        {
            var devices = await _deviceRepository.GetAllDevicesAsync();
            var deviceDtos = new List<DeviceDto>();
            
            foreach (var device in devices)
            {
                var deviceDto = await MapToDeviceDtoAsync(device);
                deviceDtos.Add(deviceDto);
            }
            
            return deviceDtos;
        }

        /// <summary>
        /// Belirtilen ID'ye sahip cihazı getirir
        /// </summary>
        /// <param name="id">Cihaz ID'si</param>
        /// <returns>Cihaz bilgileri</returns>
        /// <exception cref="Exception">Cihaz bulunamazsa fırlatılır</exception>
        public async Task<DeviceDto> GetDeviceByIdAsync(int id)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(id);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            return await MapToDeviceDtoAsync(device);
        }
        
        /// <summary>
        /// Belirtilen platforma ait tüm cihazları getirir
        /// </summary>
        /// <param name="platformId">Platform ID'si</param>
        /// <returns>Platforma ait cihaz listesi</returns>
        /// <exception cref="Exception">Platform bulunamazsa fırlatılır</exception>
        public async Task<List<DeviceDto>> GetDevicesByPlatformIdAsync(int platformId)
        {
            // Platform var mı kontrol et
            var platform = await _platformRepository.GetPlatformByIdAsync(platformId);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı.");
            }
            
            var devices = await _deviceRepository.GetDevicesByPlatformIdAsync(platformId);
            var deviceDtos = new List<DeviceDto>();
            
            foreach (var device in devices)
            {
                var deviceDto = await MapToDeviceDtoAsync(device);
                deviceDtos.Add(deviceDto);
            }
            
            return deviceDtos;
        }
        
        /// <summary>
        /// Belirtilen istasyona ait tüm cihazları getirir
        /// </summary>
        /// <param name="stationId">İstasyon ID'si</param>
        /// <returns>İstasyona ait cihaz listesi</returns>
        /// <exception cref="Exception">İstasyon bulunamazsa fırlatılır</exception>
        public async Task<List<DeviceDto>> GetDevicesByStationIdAsync(int stationId)
        {
            // İstasyon var mı kontrol et
            var station = await _stationRepository.GetStationByIdAsync(stationId);
            if (station == null)
            {
                throw new Exception("İstasyon bulunamadı.");
            }
            
            var devices = await _deviceRepository.GetDevicesByStationIdAsync(stationId);
            var deviceDtos = new List<DeviceDto>();
            
            foreach (var device in devices)
            {
                var deviceDto = await MapToDeviceDtoAsync(device);
                deviceDtos.Add(deviceDto);
            }
            
            return deviceDtos;
        }

        /// <summary>
        /// Yeni bir cihaz oluşturur
        /// </summary>
        /// <param name="request">Cihaz oluşturma isteği</param>
        /// <returns>Oluşturulan cihaz bilgileri</returns>
        /// <exception cref="Exception">Platform bulunamazsa veya IP/Port kombinasyonu kullanılıyorsa fırlatılır</exception>
        public async Task<DeviceDto> CreateDeviceAsync(CreateDeviceRequest request)
        {
            // Platform var mı kontrol et
            var platform = await _platformRepository.GetPlatformByIdAsync(request.PlatformId);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı.");
            }
            
            // IP ve port kombinasyonu zaten kullanılıyor mu kontrol et
            if (await _deviceRepository.IpPortCombinationExistsAsync(request.Ip, request.Port))
            {
                throw new Exception($"Bu IP adresi ({request.Ip}) ve port ({request.Port}) kombinasyonu zaten kullanılıyor.");
            }

            var device = new Device
            {
                Name = request.Name,
                Ip = request.Ip,
                Port = request.Port,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                PlatformId = request.PlatformId
            };

            await _deviceRepository.AddDeviceAsync(device);
            
            // DeviceSettings kaydet
            if (request.Settings != null)
            {
                var deviceSettings = new DeviceSettings
                {
                    ApnName = request.Settings.ApnName,
                    ApnUsername = request.Settings.ApnUsername,
                    ApnPassword = request.Settings.ApnPassword,
                    ServerIP = request.Settings.ServerIP,
                    TcpPort = request.Settings.TcpPort,
                    UdpPort = request.Settings.UdpPort,
                    FtpStatus = request.Settings.FtpStatus,
                    DeviceId = device.Id
                };
                
                await _deviceSettingsRepository.AddDeviceSettingsAsync(deviceSettings);
            }
            
            // İlişkileri içeren device nesnesini çek
            var createdDevice = await _deviceRepository.GetDeviceByIdAsync(device.Id);
            return await MapToDeviceDtoAsync(createdDevice);
        }

        /// <summary>
        /// Mevcut bir cihazı günceller
        /// </summary>
        /// <param name="id">Güncellenecek cihazın ID'si</param>
        /// <param name="request">Cihaz güncelleme isteği</param>
        /// <returns>Güncellenen cihaz bilgileri</returns>
        /// <exception cref="Exception">Cihaz veya platform bulunamazsa veya IP/Port kombinasyonu kullanılıyorsa fırlatılır</exception>
        public async Task<DeviceDto> UpdateDeviceAsync(int id, UpdateDeviceRequest request)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(id);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            // Platform var mı kontrol et
            var platform = await _platformRepository.GetPlatformByIdAsync(request.PlatformId);
            if (platform == null)
            {
                throw new Exception("Platform bulunamadı.");
            }
            
            // IP ve port kombinasyonu zaten kullanılıyor mu kontrol et (cihazın kendisi hariç)
            if (await _deviceRepository.IpPortCombinationExistsAsync(request.Ip, request.Port, id))
            {
                throw new Exception($"Bu IP adresi ({request.Ip}) ve port ({request.Port}) kombinasyonu zaten kullanılıyor.");
            }

            device.Name = request.Name;
            device.Ip = request.Ip;
            device.Port = request.Port;
            device.Latitude = request.Latitude;
            device.Longitude = request.Longitude;
            device.PlatformId = request.PlatformId;

            await _deviceRepository.UpdateDeviceAsync(device);
            
            // DeviceSettings güncelle
            if (request.Settings != null)
            {
                var deviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByDeviceIdAsync(id);
                
                if (deviceSettings == null)
                {
                    // Ayarlar yoksa yeni kayıt ekle
                    deviceSettings = new DeviceSettings
                    {
                        DeviceId = id,
                        ApnName = request.Settings.ApnName,
                        ApnUsername = request.Settings.ApnUsername,
                        ApnPassword = request.Settings.ApnPassword,
                        ServerIP = request.Settings.ServerIP,
                        TcpPort = request.Settings.TcpPort,
                        UdpPort = request.Settings.UdpPort,
                        FtpStatus = request.Settings.FtpStatus
                    };
                    
                    await _deviceSettingsRepository.AddDeviceSettingsAsync(deviceSettings);
                }
                else
                {
                    // Mevcut ayarları güncelle
                    deviceSettings.ApnName = request.Settings.ApnName;
                    deviceSettings.ApnUsername = request.Settings.ApnUsername;
                    deviceSettings.ApnPassword = request.Settings.ApnPassword;
                    deviceSettings.ServerIP = request.Settings.ServerIP;
                    deviceSettings.TcpPort = request.Settings.TcpPort;
                    deviceSettings.UdpPort = request.Settings.UdpPort;
                    deviceSettings.FtpStatus = request.Settings.FtpStatus;
                    
                    await _deviceSettingsRepository.UpdateDeviceSettingsAsync(deviceSettings);
                }
            }
            
            // İlişkileri içeren device nesnesini çek
            var updatedDevice = await _deviceRepository.GetDeviceByIdAsync(id);
            return await MapToDeviceDtoAsync(updatedDevice);
        }

        /// <summary>
        /// Belirtilen ID'ye sahip cihazı siler
        /// </summary>
        /// <param name="id">Silinecek cihazın ID'si</param>
        /// <exception cref="Exception">Cihaz bulunamazsa fırlatılır</exception>
        public async Task DeleteDeviceAsync(int id)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(id);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            await _deviceRepository.DeleteDeviceAsync(id);
        }

        /// <summary>
        /// Device nesnesini DeviceDto'ya dönüştürür
        /// </summary>
        /// <param name="device">Dönüştürülecek Device nesnesi</param>
        /// <returns>Dönüştürülmüş DeviceDto nesnesi</returns>
        private async Task<DeviceDto> MapToDeviceDtoAsync(Device device)
        {
            string stationName = "Bilinmiyor";
            
            if (device.Platform?.Station != null)
            {
                stationName = device.Platform.Station.Name;
            }
            
            var deviceDto = new DeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                Ip = device.Ip,
                Port = device.Port,
                Latitude = device.Latitude,
                Longitude = device.Longitude,
                PlatformId = device.PlatformId,
                PlatformStationName = stationName
            };
            
            // DeviceSettings bilgilerini ekle
            var deviceSettings = await _deviceSettingsRepository.GetDeviceSettingsByDeviceIdAsync(device.Id);
            if (deviceSettings != null)
            {
                deviceDto.Settings = new DeviceSettingsDto
                {
                    Id = deviceSettings.Id,
                    ApnName = deviceSettings.ApnName,
                    ApnUsername = deviceSettings.ApnUsername,
                    ApnPassword = deviceSettings.ApnPassword,
                    ServerIP = deviceSettings.ServerIP,
                    TcpPort = deviceSettings.TcpPort,
                    UdpPort = deviceSettings.UdpPort,
                    FtpStatus = deviceSettings.FtpStatus,
                    DeviceId = deviceSettings.DeviceId
                };
            }
            
            return deviceDto;
        }
    }
} 