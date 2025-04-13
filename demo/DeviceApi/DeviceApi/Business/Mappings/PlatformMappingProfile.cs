using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;
using System.Linq;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Platform varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class PlatformMappingProfile : Profile
    {
        public PlatformMappingProfile()
        {
            // Platform -> PlatformDto dönüşümü
            CreateMap<Platform, PlatformDto>()
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station != null ? src.Station.Name : "Bilinmiyor"))
                .ForMember(dest => dest.Devices, opt => opt.MapFrom(src => src.Devices))
                .ForMember(dest => dest.Prediction, opt => opt.MapFrom(src => src.Prediction));

            // CreatePlatformRequest -> Platform dönüşümü
            CreateMap<CreatePlatformRequest, Platform>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Station, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore())
                .ForMember(dest => dest.Prediction, opt => opt.Ignore());

            // UpdatePlatformRequest -> Platform dönüşümü
            CreateMap<UpdatePlatformRequest, Platform>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Station, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore())
                .ForMember(dest => dest.Prediction, opt => opt.Ignore());
        }
    }
} 