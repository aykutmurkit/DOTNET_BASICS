using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Periyodik Mesaj varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class PeriodicMessageMappingProfile : Profile
    {
        public PeriodicMessageMappingProfile()
        {
            // PeriodicMessage -> PeriodicMessageDto dönüşümü
            CreateMap<PeriodicMessage, PeriodicMessageDto>();

            // CreatePeriodicMessageRequest -> PeriodicMessage dönüşümü
            CreateMap<CreatePeriodicMessageRequest, PeriodicMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ForecastedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Device, opt => opt.Ignore());

            // UpdatePeriodicMessageRequest -> PeriodicMessage dönüşümü
            CreateMap<UpdatePeriodicMessageRequest, PeriodicMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ForecastedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DeviceId, opt => opt.Ignore())
                .ForMember(dest => dest.Device, opt => opt.Ignore());
        }
    }
} 