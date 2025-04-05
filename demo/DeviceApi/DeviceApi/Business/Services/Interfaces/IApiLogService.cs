using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DeviceApi.Business.Services.Interfaces
{
    public interface IApiLogService
    {
        Task LogInfoAsync(string message, HttpContext context = null);

        Task LogWarningAsync(string message, HttpContext context = null);

        Task LogErrorAsync(string message, Exception exception = null, HttpContext context = null);
    }
}