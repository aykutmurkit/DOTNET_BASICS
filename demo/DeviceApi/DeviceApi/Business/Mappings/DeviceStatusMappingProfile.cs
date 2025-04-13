using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Cihaz Durumu varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class DeviceStatusMappingProfile : Profile
    {
        public DeviceStatusMappingProfile()
        {
            // DeviceStatus -> DeviceStatusDto dönüşümü
            CreateMap<DeviceStatus, DeviceStatusDto>()
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device != null ? src.Device.Name : string.Empty));

            // CreateDeviceStatusDto -> DeviceStatus dönüşümü
            CreateMap<CreateDeviceStatusDto, DeviceStatus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Device, opt => opt.Ignore());

            // UpdateDeviceStatusDto -> DeviceStatus dönüşümü
            CreateMap<UpdateDeviceStatusDto, DeviceStatus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Device, opt => opt.Ignore());
        }
    }
} 