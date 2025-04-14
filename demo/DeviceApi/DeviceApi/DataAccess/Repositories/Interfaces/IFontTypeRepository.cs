using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Font türü repository arayüzü
    /// </summary>
    public interface IFontTypeRepository
    {
        /// <summary>
        /// Tüm font türlerini getirir
        /// </summary>
        Task<List<FontType>> GetAllFontTypesAsync();
        
        /// <summary>
        /// ID'ye göre font türü getirir
        /// </summary>
        Task<FontType> GetFontTypeByIdAsync(int id);
        
        /// <summary>
        /// Key'e göre font türü getirir
        /// </summary>
        Task<FontType> GetFontTypeByKeyAsync(int key);
        
        /// <summary>
        /// Font türü ekler
        /// </summary>
        Task AddFontTypeAsync(FontType fontType);
        
        /// <summary>
        /// Font türü günceller
        /// </summary>
        Task UpdateFontTypeAsync(FontType fontType);
        
        /// <summary>
        /// Font türü siler
        /// </summary>
        Task DeleteFontTypeAsync(int id);
    }
} 