using AutoMapper;
using Data.Interfaces;
using DeviceApi.Business.Services.Interfaces;
using Entities.Concrete;
using Entities.Dtos;
using LogLibrary.Core.Interfaces;

namespace DeviceApi.Business.Services.Concrete
{
    /// <summary>
    /// Font türü servis implementasyonu
    /// </summary>
    public class FontTypeService : IFontTypeService
    {
        private readonly IFontTypeRepository _fontTypeRepository;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;

        public FontTypeService(
            IFontTypeRepository fontTypeRepository,
            IMapper mapper,
            ILogService logService)
        {
            _fontTypeRepository = fontTypeRepository;
            _mapper = mapper;
            _logService = logService;
        }

        public async Task<List<FontTypeDto>> GetAllFontTypesAsync()
        {
            var fontTypes = await _fontTypeRepository.GetAllFontTypesAsync();
            return _mapper.Map<List<FontTypeDto>>(fontTypes);
        }

        public async Task<FontTypeDto> GetFontTypeByIdAsync(int id)
        {
            var fontType = await _fontTypeRepository.GetFontTypeByIdAsync(id);
            if (fontType == null)
            {
                throw new Exception("Font türü bulunamadı.");
            }

            return _mapper.Map<FontTypeDto>(fontType);
        }

        public async Task<FontTypeDto> GetFontTypeByKeyAsync(int key)
        {
            var fontType = await _fontTypeRepository.GetFontTypeByKeyAsync(key);
            if (fontType == null)
            {
                throw new Exception("Font türü bulunamadı.");
            }

            return _mapper.Map<FontTypeDto>(fontType);
        }

        public async Task<FontTypeDto> CreateFontTypeAsync(CreateFontTypeRequest request)
        {
            // Aynı Key değerine sahip başka bir kayıt var mı kontrol et
            var existingFontType = await _fontTypeRepository.GetFontTypeByKeyAsync(request.Key);
            if (existingFontType != null)
            {
                throw new Exception($"Bu Key değerine ({request.Key}) sahip bir font türü zaten mevcut.");
            }

            var fontType = _mapper.Map<FontType>(request);
            await _fontTypeRepository.AddFontTypeAsync(fontType);
            
            await _logService.LogInfoAsync(
                "Font türü oluşturuldu",
                "FontTypeService.CreateFontTypeAsync",
                new { FontTypeId = fontType.Id, FontTypeKey = fontType.Key, FontTypeName = fontType.Name });

            return _mapper.Map<FontTypeDto>(fontType);
        }

        public async Task<FontTypeDto> UpdateFontTypeAsync(int id, UpdateFontTypeRequest request)
        {
            var fontType = await _fontTypeRepository.GetFontTypeByIdAsync(id);
            if (fontType == null)
            {
                throw new Exception("Font türü bulunamadı.");
            }

            // Aynı Key değerine sahip başka bir kayıt var mı kontrol et (kendisi hariç)
            var existingFontType = await _fontTypeRepository.GetFontTypeByKeyAsync(request.Key);
            if (existingFontType != null && existingFontType.Id != id)
            {
                throw new Exception($"Bu Key değerine ({request.Key}) sahip başka bir font türü zaten mevcut.");
            }

            _mapper.Map(request, fontType);
            await _fontTypeRepository.UpdateFontTypeAsync(fontType);
            
            await _logService.LogInfoAsync(
                "Font türü güncellendi",
                "FontTypeService.UpdateFontTypeAsync",
                new { FontTypeId = fontType.Id, FontTypeKey = fontType.Key, FontTypeName = fontType.Name });

            return _mapper.Map<FontTypeDto>(fontType);
        }

        public async Task DeleteFontTypeAsync(int id)
        {
            var fontType = await _fontTypeRepository.GetFontTypeByIdAsync(id);
            if (fontType == null)
            {
                throw new Exception("Font türü bulunamadı.");
            }

            // İlişkili FullScreenMessage'lar var mı kontrol et
            if (fontType.FullScreenMessages != null && fontType.FullScreenMessages.Any())
            {
                throw new Exception("Bu font türü kullanımda olduğu için silinemez. Önce ilişkili mesajları kaldırın.");
            }

            await _fontTypeRepository.DeleteFontTypeAsync(id);
            
            await _logService.LogInfoAsync(
                "Font türü silindi",
                "FontTypeService.DeleteFontTypeAsync",
                new { FontTypeId = id });
        }
    }
} 