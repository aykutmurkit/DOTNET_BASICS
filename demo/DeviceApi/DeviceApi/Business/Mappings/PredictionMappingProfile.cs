using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Tren Tahmin varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class PredictionMappingProfile : Profile
    {
        public PredictionMappingProfile()
        {
            // Prediction -> PredictionDto dönüşümü
            CreateMap<Prediction, PredictionDto>();

            // CreatePredictionRequest -> Prediction dönüşümü
            CreateMap<CreatePredictionRequest, Prediction>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ForecastGenerationAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Platform, opt => opt.Ignore());

            // UpdatePredictionRequest -> Prediction dönüşümü
            CreateMap<UpdatePredictionRequest, Prediction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PlatformId, opt => opt.Ignore())
                .ForMember(dest => dest.Platform, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ForecastGenerationAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
} 