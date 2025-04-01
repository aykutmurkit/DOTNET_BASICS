using System;
using System.Threading.Tasks;
using LogAPI.Entities.Models;

namespace LogAPI.Business.Services.Logging
{
    /// <summary>
    /// Log veritabanı işlemleri için repository interface'i
    /// </summary>
    public interface ILogRepository
    {
        /// <summary>
        /// İstek/Yanıt loglarını sayfalı şekilde getirir
        /// </summary>
        /// <param name="pageNumber">Sayfa numarası</param>
        /// <param name="pageSize">Sayfa başına kayıt sayısı</param>
        /// <param name="search">Arama terimi (isteğe bağlı)</param>
        /// <returns>Sayfalanmış istek/yanıt logları</returns>
        Task<PagedResult<RequestResponseLog>> GetRequestResponseLogsAsync(int pageNumber, int pageSize, string search = null);
        
        /// <summary>
        /// API loglarını sayfalı şekilde getirir
        /// </summary>
        /// <param name="pageNumber">Sayfa numarası</param>
        /// <param name="pageSize">Sayfa başına kayıt sayısı</param>
        /// <param name="level">Log seviyesi filtresi (isteğe bağlı)</param>
        /// <param name="search">Arama terimi (isteğe bağlı)</param>
        /// <returns>Sayfalanmış API logları</returns>
        Task<PagedResult<ApiLog>> GetApiLogsAsync(int pageNumber, int pageSize, string level = null, string search = null);
        
        /// <summary>
        /// Belirli bir istek/yanıt logunu ID'ye göre getirir
        /// </summary>
        /// <param name="id">Log ID</param>
        /// <returns>İstek/yanıt log detayı</returns>
        Task<RequestResponseLog> GetRequestResponseLogByIdAsync(string id);
        
        /// <summary>
        /// Belirli bir API logunu ID'ye göre getirir
        /// </summary>
        /// <param name="id">Log ID</param>
        /// <returns>API log detayı</returns>
        Task<ApiLog> GetApiLogByIdAsync(string id);
        
        /// <summary>
        /// Belirtilen kriterlere göre istek/yanıt loglarını siler
        /// </summary>
        /// <param name="olderThan">Bu tarihten eski logları sil (isteğe bağlı)</param>
        /// <param name="path">Belirli bir URL yoluna sahip logları sil (isteğe bağlı)</param>
        /// <returns>Silinen log sayısı</returns>
        Task<long> DeleteRequestResponseLogsAsync(DateTime? olderThan = null, string path = null);
        
        /// <summary>
        /// Belirtilen kriterlere göre API loglarını siler
        /// </summary>
        /// <param name="olderThan">Bu tarihten eski logları sil (isteğe bağlı)</param>
        /// <param name="level">Belirli seviyedeki logları sil (isteğe bağlı)</param>
        /// <returns>Silinen log sayısı</returns>
        Task<long> DeleteApiLogsAsync(DateTime? olderThan = null, string level = null);
        
        /// <summary>
        /// Tüm log kayıtlarını siler
        /// </summary>
        /// <returns>Silinen toplam log sayısı</returns>
        Task<long> DeleteAllLogsAsync();
    }
} 