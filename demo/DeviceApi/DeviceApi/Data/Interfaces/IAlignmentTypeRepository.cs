using Entities.Concrete;

namespace Data.Interfaces
{
    /// <summary>
    /// Hizalama türleri için repository interface
    /// </summary>
    public interface IAlignmentTypeRepository
    {
        /// <summary>
        /// Tüm hizalama türlerini getirir
        /// </summary>
        Task<List<AlignmentType>> GetAllAlignmentTypesAsync();

        /// <summary>
        /// ID'ye göre hizalama türü getirir
        /// </summary>
        Task<AlignmentType> GetAlignmentTypeByIdAsync(int id);

        /// <summary>
        /// Key değerine göre hizalama türü getirir
        /// </summary>
        Task<AlignmentType> GetAlignmentTypeByKeyAsync(int key);

        /// <summary>
        /// Hizalama türü ekler
        /// </summary>
        Task AddAlignmentTypeAsync(AlignmentType alignmentType);

        /// <summary>
        /// Hizalama türü günceller
        /// </summary>
        Task UpdateAlignmentTypeAsync(AlignmentType alignmentType);

        /// <summary>
        /// Hizalama türü siler
        /// </summary>
        Task DeleteAlignmentTypeAsync(int id);
    }
} 