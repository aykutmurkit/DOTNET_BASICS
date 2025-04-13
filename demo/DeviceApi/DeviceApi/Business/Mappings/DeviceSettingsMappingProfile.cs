using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace DeviceApi.Business.Mappings
{
    /// <summary>
    /// Cihaz Ayarları varlığı için AutoMapper profil yapılandırması
    /// </summary>
    public class DeviceSettingsMappingProfile : Profile
    {
        public DeviceSettingsMappingProfile()
        {
            // DeviceSettings -> DeviceSettingsDto dönüşümü
            CreateMap<DeviceSettings, DeviceSettingsDto>();

            // CreateDeviceSettingsRequest -> DeviceSettings dönüşümü
            CreateMap<CreateDeviceSettingsRequest, DeviceSettings>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceId, opt => opt.Ignore())
                .ForMember(dest => dest.Device, opt => opt.Ignore());

            // UpdateDeviceSettingsRequest -> DeviceSettings dönüşümü
            CreateMap<UpdateDeviceSettingsRequest, DeviceSettings>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceId, opt => opt.Ignore())
                .ForMember(dest => dest.Device, opt => opt.Ignore());
        }
    }
} 