using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Tam Ekran Mesaj varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class FullScreenMessageMappingProfile : Profile
    {
        public FullScreenMessageMappingProfile()
        {
            // AlignmentType -> AlignmentValueDto dönüşümü
            CreateMap<AlignmentType, AlignmentValueDto>();

            // FullScreenMessage -> FullScreenMessageDto dönüşümü
            CreateMap<FullScreenMessage, FullScreenMessageDto>()
                .ForMember(dest => dest.Alignment, opt => opt.MapFrom(src => src.AlignmentType))
                .ForMember(dest => dest.DeviceIds, opt => opt.MapFrom(src => 
                    src.Devices != null ? src.Devices.Select(d => d.Id).ToList() : new List<int>()));

            // CreateFullScreenMessageRequest -> FullScreenMessage dönüşümü
            CreateMap<CreateFullScreenMessageRequest, FullScreenMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.AlignmentType, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore());

            // UpdateFullScreenMessageRequest -> FullScreenMessage dönüşümü
            CreateMap<UpdateFullScreenMessageRequest, FullScreenMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.AlignmentType, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore());
        }
    }
} 