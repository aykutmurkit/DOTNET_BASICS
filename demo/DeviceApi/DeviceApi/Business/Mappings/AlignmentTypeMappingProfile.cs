using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Hizalama türleri için AutoMapper profil yapılandırması
    /// </summary>
    public class AlignmentTypeMappingProfile : Profile
    {
        public AlignmentTypeMappingProfile()
        {
            // AlignmentType -> AlignmentTypeDto dönüşümü
            CreateMap<AlignmentType, AlignmentTypeDto>();
            
            // AlignmentType -> AlignmentValueDto dönüşümü (FullScreenMessageDto içinde kullanılıyor)
            CreateMap<AlignmentType, AlignmentValueDto>();
            
            // CreateAlignmentTypeRequest -> AlignmentType dönüşümü
            CreateMap<CreateAlignmentTypeRequest, AlignmentType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FullScreenMessages, opt => opt.Ignore());
                
            // UpdateAlignmentTypeRequest -> AlignmentType dönüşümü
            CreateMap<UpdateAlignmentTypeRequest, AlignmentType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FullScreenMessages, opt => opt.Ignore());
        }
    }
} 