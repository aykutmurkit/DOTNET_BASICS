using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Cihaz varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class DeviceMappingProfile : Profile
    {
        public DeviceMappingProfile()
        {
            // Device -> DeviceDto dönüşümü
            CreateMap<Device, DeviceDto>()
                .ForMember(dest => dest.PlatformStationName, opt => 
                    opt.MapFrom(src => src.Platform != null && src.Platform.Station != null 
                        ? src.Platform.Station.Name 
                        : "Bilinmiyor"));

            // CreateDeviceRequest -> Device dönüşümü
            CreateMap<CreateDeviceRequest, Device>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Platform, opt => opt.Ignore())
                .ForMember(dest => dest.Settings, opt => opt.Ignore()) 
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.FullScreenMessage, opt => opt.Ignore())
                .ForMember(dest => dest.ScrollingScreenMessage, opt => opt.Ignore())
                .ForMember(dest => dest.BitmapScreenMessage, opt => opt.Ignore())
                .ForMember(dest => dest.PeriodicMessage, opt => opt.Ignore());

            // UpdateDeviceRequest -> Device dönüşümü
            CreateMap<UpdateDeviceRequest, Device>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Platform, opt => opt.Ignore())
                .ForMember(dest => dest.Settings, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.FullScreenMessage, opt => opt.Ignore())
                .ForMember(dest => dest.ScrollingScreenMessage, opt => opt.Ignore())
                .ForMember(dest => dest.BitmapScreenMessage, opt => opt.Ignore())
                .ForMember(dest => dest.PeriodicMessage, opt => opt.Ignore());
                
            // Device Settings Mappings
            CreateMap<DeviceSettings, DeviceSettingsDto>();
            CreateMap<CreateDeviceSettingsRequest, DeviceSettings>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceId, opt => opt.Ignore());
            CreateMap<UpdateDeviceSettingsRequest, DeviceSettings>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceId, opt => opt.Ignore());
                
            // Device Status Mappings
            CreateMap<DeviceStatus, DeviceStatusDto>()
                .ForMember(dest => dest.DeviceName, opt => 
                    opt.MapFrom(src => src.Device != null ? src.Device.Name : null));
            CreateMap<CreateDeviceStatusDto, DeviceStatus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Device, opt => opt.Ignore());
            CreateMap<UpdateDeviceStatusDto, DeviceStatus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Device, opt => opt.Ignore());
                
            // FullScreenMessage Mappings
            CreateMap<FullScreenMessage, FullScreenMessageDto>()
                .ForMember(dest => dest.DeviceIds, opt => 
                    opt.MapFrom(src => src.Devices != null ? src.Devices.Select(d => d.Id).ToList() : new List<int>()))
                .ForMember(dest => dest.Alignment, opt => 
                    opt.MapFrom(src => src.AlignmentType != null ? new AlignmentValueDto 
                    { 
                        Id = src.AlignmentType.Id, 
                        Key = src.AlignmentType.Key, 
                        Name = src.AlignmentType.Name 
                    } : null));
            CreateMap<CreateFullScreenMessageRequest, FullScreenMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.AlignmentType, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore());
            CreateMap<UpdateFullScreenMessageRequest, FullScreenMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.AlignmentType, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore());
                
            // ScrollingScreenMessage Mappings
            CreateMap<ScrollingScreenMessage, ScrollingScreenMessageDto>()
                .ForMember(dest => dest.DeviceIds, opt => 
                    opt.MapFrom(src => src.Devices != null ? src.Devices.Select(d => d.Id).ToList() : new List<int>()));
            CreateMap<CreateScrollingScreenMessageRequest, ScrollingScreenMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore());
            CreateMap<UpdateScrollingScreenMessageRequest, ScrollingScreenMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Devices, opt => opt.Ignore());
                
            // BitmapScreenMessage Mappings
            CreateMap<BitmapScreenMessage, BitmapScreenMessageDto>()
                .ForMember(dest => dest.DeviceIds, opt => 
                    opt.MapFrom(src => src.Devices != null ? src.Devices.Select(d => d.Id).ToList() : new List<int>()));
            CreateMap<CreateBitmapScreenMessageRequest, BitmapScreenMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore());
            CreateMap<UpdateBitmapScreenMessageRequest, BitmapScreenMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Devices, opt => opt.Ignore());
                
            // PeriodicMessage Mappings
            CreateMap<PeriodicMessage, PeriodicMessageDto>();
            CreateMap<CreatePeriodicMessageRequest, PeriodicMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ForecastedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Device, opt => opt.Ignore());
            CreateMap<UpdatePeriodicMessageRequest, PeriodicMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ForecastedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Device, opt => opt.Ignore());
        }
    }
} 