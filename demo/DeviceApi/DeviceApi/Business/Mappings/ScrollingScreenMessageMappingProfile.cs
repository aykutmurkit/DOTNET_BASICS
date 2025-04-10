using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;
using System.Linq;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Kayan Ekran Mesajı varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class ScrollingScreenMessageMappingProfile : Profile
    {
        public ScrollingScreenMessageMappingProfile()
        {
            // Kayan Ekran Mesajı -> Kayan Ekran Mesajı DTO dönüşümü
            CreateMap<ScrollingScreenMessage, ScrollingScreenMessageDto>()
                .ForMember(dest => dest.DeviceIds, opt => opt.MapFrom(src => 
                    src.Devices != null ? src.Devices.Select(d => d.Id).ToList() : new List<int>()));

            // Kayan Ekran Mesajı Oluşturma İsteği -> Kayan Ekran Mesajı dönüşümü
            CreateMap<CreateScrollingScreenMessageRequest, ScrollingScreenMessage>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore());

            // Kayan Ekran Mesajı Güncelleme İsteği -> Kayan Ekran Mesajı dönüşümü
            CreateMap<UpdateScrollingScreenMessageRequest, ScrollingScreenMessage>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Devices, opt => opt.Ignore());
        }
    }
} 