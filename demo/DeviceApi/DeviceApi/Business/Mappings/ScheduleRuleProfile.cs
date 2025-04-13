using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Zamanlanmış kurallar için AutoMapper profili
    /// </summary>
    public class ScheduleRuleProfile : Profile
    {
        public ScheduleRuleProfile()
        {
            // Entity -> DTO
            CreateMap<ScheduleRule, ScheduleRuleDto>()
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device != null ? src.Device.Name : null));
                
            // DTO -> Entity
            CreateMap<CreateScheduleRuleDto, ScheduleRule>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Device, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
                
            CreateMap<UpdateScheduleRuleDto, ScheduleRule>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Device, opt => opt.Ignore());
        }
    }
} 