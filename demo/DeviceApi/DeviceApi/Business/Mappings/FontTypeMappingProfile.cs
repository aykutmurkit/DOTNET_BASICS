using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Font türü varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class FontTypeMappingProfile : Profile
    {
        public FontTypeMappingProfile()
        {
            // FontType -> FontTypeDto dönüşümü
            CreateMap<FontType, FontTypeDto>();
            
            // FontType -> FontTypeValueDto dönüşümü
            CreateMap<FontType, FontTypeValueDto>();
            
            // CreateFontTypeRequest -> FontType dönüşümü
            CreateMap<CreateFontTypeRequest, FontType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FullScreenMessages, opt => opt.Ignore());
            
            // UpdateFontTypeRequest -> FontType dönüşümü
            CreateMap<UpdateFontTypeRequest, FontType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FullScreenMessages, opt => opt.Ignore());
        }
    }
} 