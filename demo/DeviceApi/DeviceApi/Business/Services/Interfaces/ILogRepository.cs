using System;
using System.Threading.Tasks;
using DeviceApi.Models.Logs;

namespace DeviceApi.Business.Services.Interfaces
{
    public interface ILogRepository
    {
        Task SaveRequestResponseLogAsync(RequestResponseLog log);

        Task SaveApiLogAsync(ApiLog log);

        Task<PagedResult<RequestResponseLog>> GetRequestResponseLogsAsync(int pageNumber, int pageSize, string search = null);

        Task<PagedResult<ApiLog>> GetApiLogsAsync(int pageNumber, int pageSize, string level = null, string search = null);

        // Log temizleme metotları
        Task<long> DeleteRequestResponseLogsAsync(DateTime? olderThan = null, string path = null);

        Task<long> DeleteApiLogsAsync(DateTime? olderThan = null, string level = null);

        Task<long> DeleteAllLogsAsync();
    }
}