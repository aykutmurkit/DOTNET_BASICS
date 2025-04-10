using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// İstasyon varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class StationMappingProfile : Profile
    {
        public StationMappingProfile()
        {
            // İstasyon -> İstasyonDto dönüşümü
            CreateMap<Station, StationDto>()
                .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src => src.Platforms));

            // Platform -> PlatformDto dönüşümü
            CreateMap<Platform, PlatformDto>()
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.Name))
                .ForMember(dest => dest.Devices, opt => opt.MapFrom(src => src.Devices));

            // Cihaz -> CihazDto dönüşümü
            CreateMap<Device, DeviceDto>()
                .ForMember(dest => dest.PlatformStationName, opt => opt.MapFrom(src => src.Platform.Station.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // CihazDurumu -> CihazDurumuDto dönüşümü
            CreateMap<DeviceStatus, DeviceStatusDto>()
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device.Name));

            // İstasyonOluşturmaİsteği -> İstasyon dönüşümü
            CreateMap<CreateStationRequest, Station>();

            // İstasyonGüncellemeİsteği -> İstasyon dönüşümü
            CreateMap<UpdateStationRequest, Station>();
        }
    }
}