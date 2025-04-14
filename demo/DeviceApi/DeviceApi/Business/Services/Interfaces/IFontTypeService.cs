using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Font türü servis arayüzü
    /// </summary>
    public interface IFontTypeService
    {
        /// <summary>
        /// Tüm font türlerini getirir
        /// </summary>
        Task<List<FontTypeDto>> GetAllFontTypesAsync();
        
        /// <summary>
        /// ID'ye göre font türü getirir
        /// </summary>
        Task<FontTypeDto> GetFontTypeByIdAsync(int id);

        /// <summary>
        /// Key'e göre font türü getirir
        /// </summary>
        Task<FontTypeDto> GetFontTypeByKeyAsync(int key);
        
        /// <summary>
        /// Font türü oluşturur
        /// </summary>
        Task<FontTypeDto> CreateFontTypeAsync(CreateFontTypeRequest request);
        
        /// <summary>
        /// Font türü günceller
        /// </summary>
        Task<FontTypeDto> UpdateFontTypeAsync(int id, UpdateFontTypeRequest request);
        
        /// <summary>
        /// Font türü siler
        /// </summary>
        Task DeleteFontTypeAsync(int id);
    }
} 