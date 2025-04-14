using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Tam ekran mesaj varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class FullScreenMessageMappingProfile : Profile
    {
        public FullScreenMessageMappingProfile()
        {
            // FullScreenMessage -> FullScreenMessageDto dönüşümü
            CreateMap<FullScreenMessage, FullScreenMessageDto>()
                .ForMember(dest => dest.DeviceIds, opt => 
                    opt.MapFrom(src => src.Devices != null ? src.Devices.Select(d => d.Id).ToList() : new List<int>()))
                .ForMember(dest => dest.Alignment, opt => 
                    opt.MapFrom(src => src.AlignmentType != null ? new AlignmentValueDto 
                    { 
                        Id = src.AlignmentType.Id, 
                        Key = src.AlignmentType.Key, 
                        Name = src.AlignmentType.Name 
                    } : null))
                .ForMember(dest => dest.FontType, opt => 
                    opt.MapFrom(src => src.FontType != null ? new FontTypeValueDto 
                    { 
                        Id = src.FontType.Id, 
                        Key = src.FontType.Key, 
                        Name = src.FontType.Name 
                    } : null));
            
            // CreateFullScreenMessageRequest -> FullScreenMessage dönüşümü
            CreateMap<CreateFullScreenMessageRequest, FullScreenMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.AlignmentType, opt => opt.Ignore())
                .ForMember(dest => dest.FontType, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore());
            
            // UpdateFullScreenMessageRequest -> FullScreenMessage dönüşümü
            CreateMap<UpdateFullScreenMessageRequest, FullScreenMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.AlignmentType, opt => opt.Ignore())
                .ForMember(dest => dest.FontType, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore());
        }
    }
} 