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

            // İstasyonOluşturmaİsteği -> İstasyon dönüşümü
            CreateMap<CreateStationRequest, Station>();

            // İstasyonGüncellemeİsteği -> İstasyon dönüşümü
            CreateMap<UpdateStationRequest, Station>();
        }
    }
}