using AutoMapper;
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
        private readonly IFullScreenMessageRepository _fullScreenMessageRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// DeviceService sınıfının constructor'ı
        /// </summary>
        /// <param name="deviceRepository">Cihaz veritabanı işlemleri için repository</param>
        /// <param name="platformRepository">Platform veritabanı işlemleri için repository</param>
        /// <param name="stationRepository">İstasyon veritabanı işlemleri için repository</param>
        /// <param name="deviceSettingsRepository">Cihaz ayarları veritabanı işlemleri için repository</param>
        /// <param name="fullScreenMessageRepository">Tam ekran mesaj veritabanı işlemleri için repository</param>
        /// <param name="mapper">AutoMapper instance</param>
        public DeviceService(
            IDeviceRepository deviceRepository, 
            IPlatformRepository platformRepository, 
            IStationRepository stationRepository,
            IDeviceSettingsRepository deviceSettingsRepository,
            IFullScreenMessageRepository fullScreenMessageRepository,
            IMapper mapper)
        {
            _deviceRepository = deviceRepository;
            _platformRepository = platformRepository;
            _stationRepository = stationRepository;
            _deviceSettingsRepository = deviceSettingsRepository;
            _fullScreenMessageRepository = fullScreenMessageRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Tüm cihazları listeler
        /// </summary>
        /// <returns>Cihaz listesi</returns>
        public async Task<List<DeviceDto>> GetAllDevicesAsync()
        {
            var devices = await _deviceRepository.GetAllDevicesAsync();
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        /// <summary>
        /// ID'ye göre cihaz getirir
        /// </summary>
        /// <param name="id">Cihaz ID</param>
        /// <returns>Cihaz bilgisi</returns>
        public async Task<DeviceDto> GetDeviceByIdAsync(int id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            return _mapper.Map<DeviceDto>(device);
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
            return _mapper.Map<List<DeviceDto>>(devices);
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
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        /// <summary>
        /// İsme göre cihazları filtreler
        /// </summary>
        /// <param name="name">Cihaz adı</param>
        /// <returns>Filtrelenmiş cihaz listesi</returns>
        public async Task<List<DeviceDto>> GetDevicesByNameAsync(string name)
        {
            var devices = await _deviceRepository.GetDevicesByNameAsync(name);
            return _mapper.Map<List<DeviceDto>>(devices);
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

            var device = _mapper.Map<Device>(request);

            await _deviceRepository.AddDeviceAsync(device);
            
            // DeviceSettings kaydet
            if (request.Settings != null)
            {
                var deviceSettings = _mapper.Map<DeviceSettings>(request.Settings);
                deviceSettings.DeviceId = device.Id;
                
                await _deviceSettingsRepository.AddDeviceSettingsAsync(deviceSettings);
            }
            
            // FullScreenMessage ekle
            if (request.FullScreenMessage != null)
            {
                var fullScreenMessage = _mapper.Map<FullScreenMessage>(request.FullScreenMessage);
                fullScreenMessage.CreatedAt = DateTime.Now;
                
                await _fullScreenMessageRepository.AddFullScreenMessageAsync(fullScreenMessage);
                
                // Mesajı cihaza ata
                device.FullScreenMessageId = fullScreenMessage.Id;
                await _deviceRepository.UpdateDeviceAsync(device);
            }
            
            // İlişkileri içeren device nesnesini çek
            var createdDevice = await _deviceRepository.GetDeviceByIdAsync(device.Id);
            return _mapper.Map<DeviceDto>(createdDevice);
        }

        /// <summary>
        /// Cihaz bilgilerini günceller
        /// </summary>
        /// <param name="id">Güncellenecek cihaz ID</param>
        /// <param name="request">Güncelleme bilgileri</param>
        /// <returns>Güncellenmiş cihaz bilgileri</returns>
        /// <exception cref="Exception">Cihaz veya platform bulunamazsa veya IP/Port kombinasyonu başka bir cihaz tarafından kullanılıyorsa fırlatılır</exception>
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
            
            // IP ve port kombinasyonu başka bir cihaz için var mı kontrol et
            if (await _deviceRepository.IpPortCombinationExistsForDifferentDeviceAsync(id, request.Ip, request.Port))
            {
                throw new Exception($"Bu IP adresi ({request.Ip}) ve port ({request.Port}) kombinasyonu zaten başka bir cihaz tarafından kullanılıyor.");
            }
            
            // Cihaz bilgilerini güncelle
            _mapper.Map(request, device);
            
            await _deviceRepository.UpdateDeviceAsync(device);
            
            // DeviceSettings güncelle
            if (request.Settings != null)
            {
                var deviceSettings = await _deviceSettingsRepository.GetSettingsByDeviceIdAsync(id);
                if (deviceSettings != null)
                {
                    _mapper.Map(request.Settings, deviceSettings);
                    await _deviceSettingsRepository.UpdateDeviceSettingsAsync(deviceSettings);
                }
                else
                {
                    // Eğer ayarlar yoksa yeni ekle
                    var newSettings = _mapper.Map<DeviceSettings>(request.Settings);
                    newSettings.DeviceId = id;
                    await _deviceSettingsRepository.AddDeviceSettingsAsync(newSettings);
                }
            }
            
            // FullScreenMessage güncelle
            if (request.FullScreenMessage != null)
            {
                if (device.FullScreenMessageId.HasValue)
                {
                    var message = await _fullScreenMessageRepository.GetFullScreenMessageByIdAsync(device.FullScreenMessageId.Value);
                    if (message != null)
                    {
                        _mapper.Map(request.FullScreenMessage, message);
                        await _fullScreenMessageRepository.UpdateFullScreenMessageAsync(message);
                    }
                }
                else
                {
                    // Eğer mesaj yoksa yeni ekle
                    var newMessage = _mapper.Map<FullScreenMessage>(request.FullScreenMessage);
                    newMessage.CreatedAt = DateTime.Now;
                    
                    await _fullScreenMessageRepository.AddFullScreenMessageAsync(newMessage);
                    
                    // Mesajı cihaza ata
                    device.FullScreenMessageId = newMessage.Id;
                    await _deviceRepository.UpdateDeviceAsync(device);
                }
            }
            
            // Güncellenmiş cihazı çek
            var updatedDevice = await _deviceRepository.GetDeviceByIdAsync(id);
            return _mapper.Map<DeviceDto>(updatedDevice);
        }

        /// <summary>
        /// Cihazı ve ilişkili kayıtları siler
        /// </summary>
        /// <param name="id">Silinecek cihaz ID</param>
        /// <exception cref="Exception">Cihaz bulunamazsa fırlatılır</exception>
        public async Task DeleteDeviceAsync(int id)
        {
            var device = await _deviceRepository.GetDeviceByIdAsync(id);
            if (device == null)
            {
                throw new Exception("Cihaz bulunamadı.");
            }

            // İlişkili FullScreenMessage varsa önce onu sil
            if (device.FullScreenMessageId.HasValue)
            {
                await _fullScreenMessageRepository.DeleteFullScreenMessageAsync(device.FullScreenMessageId.Value);
            }

            // İlişkili DeviceSettings'i sil
            await _deviceSettingsRepository.DeleteDeviceSettingsByDeviceIdAsync(id);

            // Cihazı sil
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
            if (device.Settings != null)
            {
                deviceDto.Settings = new DeviceSettingsDto
                {
                    Id = device.Settings.Id,
                    ApnName = device.Settings.ApnName,
                    ApnUsername = device.Settings.ApnUsername,
                    ApnPassword = device.Settings.ApnPassword,
                    ServerIP = device.Settings.ServerIP,
                    TcpPort = device.Settings.TcpPort,
                    UdpPort = device.Settings.UdpPort,
                    FtpStatus = device.Settings.FtpStatus,
                    DeviceId = device.Settings.DeviceId
                };
            }
            
            // DeviceStatus bilgilerini ekle
            if (device.Status != null)
            {
                deviceDto.Status = new DeviceStatusDto
                {
                    Id = device.Status.Id,
                    FullScreenMessageStatus = device.Status.FullScreenMessageStatus,
                    ScrollingScreenMessageStatus = device.Status.ScrollingScreenMessageStatus,
                    BitmapScreenMessageStatus = device.Status.BitmapScreenMessageStatus,
                    DeviceId = device.Status.DeviceId,
                    DeviceName = device.Name,
                    CreatedAt = device.Status.CreatedAt,
                    UpdatedAt = device.Status.UpdatedAt
                };
            }
            
            // FullScreenMessage bilgilerini ekle
            if (device.FullScreenMessage != null)
            {
                deviceDto.FullScreenMessage = new FullScreenMessageDto
                {
                    Id = device.FullScreenMessage.Id,
                    TurkishLine1 = device.FullScreenMessage.TurkishLine1,
                    TurkishLine2 = device.FullScreenMessage.TurkishLine2,
                    TurkishLine3 = device.FullScreenMessage.TurkishLine3,
                    TurkishLine4 = device.FullScreenMessage.TurkishLine4,
                    EnglishLine1 = device.FullScreenMessage.EnglishLine1,
                    EnglishLine2 = device.FullScreenMessage.EnglishLine2,
                    EnglishLine3 = device.FullScreenMessage.EnglishLine3,
                    EnglishLine4 = device.FullScreenMessage.EnglishLine4,
                    CreatedAt = device.FullScreenMessage.CreatedAt,
                    ModifiedAt = device.FullScreenMessage.ModifiedAt,
                    DeviceIds = new List<int> { device.Id }
                };
            }
            
            // ScrollingScreenMessage bilgilerini ekle
            if (device.ScrollingScreenMessage != null)
            {
                deviceDto.ScrollingScreenMessage = new ScrollingScreenMessageDto
                {
                    Id = device.ScrollingScreenMessage.Id,
                    TurkishLine = device.ScrollingScreenMessage.TurkishLine,
                    EnglishLine = device.ScrollingScreenMessage.EnglishLine,
                    CreatedAt = device.ScrollingScreenMessage.CreatedAt,
                    UpdatedAt = device.ScrollingScreenMessage.UpdatedAt,
                    DeviceIds = new List<int> { device.Id }
                };
            }
            
            // BitmapScreenMessage bilgilerini ekle
            if (device.BitmapScreenMessage != null)
            {
                deviceDto.BitmapScreenMessage = new BitmapScreenMessageDto
                {
                    Id = device.BitmapScreenMessage.Id,
                    TurkishBitmap = device.BitmapScreenMessage.TurkishBitmap,
                    EnglishBitmap = device.BitmapScreenMessage.EnglishBitmap,
                    CreatedAt = device.BitmapScreenMessage.CreatedAt,
                    UpdatedAt = device.BitmapScreenMessage.UpdatedAt,
                    DeviceIds = new List<int> { device.Id }
                };
            }
            
            // PeriodicMessage bilgilerini ekle
            if (device.PeriodicMessage != null)
            {
                deviceDto.PeriodicMessage = new PeriodicMessageDto
                {
                    Id = device.PeriodicMessage.Id,
                    TemperatureLevel = device.PeriodicMessage.TemperatureLevel,
                    HumidityLevel = device.PeriodicMessage.HumidityLevel,
                    GasLevel = device.PeriodicMessage.GasLevel,
                    FrontLightLevel = device.PeriodicMessage.FrontLightLevel,
                    BackLightLevel = device.PeriodicMessage.BackLightLevel,
                    LedFailureCount = device.PeriodicMessage.LedFailureCount,
                    CabinStatus = device.PeriodicMessage.CabinStatus,
                    FanStatus = device.PeriodicMessage.FanStatus,
                    ShowStatus = device.PeriodicMessage.ShowStatus,
                    Rs232Status = device.PeriodicMessage.Rs232Status,
                    PowerSupplyStatus = device.PeriodicMessage.PowerSupplyStatus,
                    CreatedAt = device.PeriodicMessage.CreatedAt,
                    ForecastedAt = device.PeriodicMessage.ForecastedAt,
                    DeviceId = device.PeriodicMessage.DeviceId
                };
            }
            
            return deviceDto;
        }
    }
} 