using Entities.Dtos;

namespace DeviceApi.Business.Services.Interfaces
{
    /// <summary>
    /// Hizalama türleri servisi için interface
    /// </summary>
    public interface IAlignmentTypeService
    {
        /// <summary>
        /// Tüm hizalama türlerini getirir
        /// </summary>
        Task<List<AlignmentTypeDto>> GetAllAlignmentTypesAsync();

        /// <summary>
        /// ID'ye göre hizalama türü getirir
        /// </summary>
        Task<AlignmentTypeDto> GetAlignmentTypeByIdAsync(int id);

        /// <summary>
        /// Key değerine göre hizalama türü getirir
        /// </summary>
        Task<AlignmentTypeDto> GetAlignmentTypeByKeyAsync(int key);

        /// <summary>
        /// Hizalama türü oluşturur
        /// </summary>
        Task<AlignmentTypeDto> CreateAlignmentTypeAsync(CreateAlignmentTypeRequest request);

        /// <summary>
        /// Hizalama türü günceller
        /// </summary>
        Task<AlignmentTypeDto> UpdateAlignmentTypeAsync(int id, UpdateAlignmentTypeRequest request);

        /// <summary>
        /// Hizalama türü siler
        /// </summary>
        Task DeleteAlignmentTypeAsync(int id);
    }
} 