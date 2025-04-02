using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace AuthenticationApi.Core.Logging
{
    public interface ILogService
    {
        void LogInfo(string message, Dictionary<string, object> additionalData = null);
        void LogWarning(string message, Dictionary<string, object> additionalData = null);
        void LogError(string message, Exception ex = null, Dictionary<string, object> additionalData = null);
        void LogDebug(string message, Dictionary<string, object> additionalData = null);
        
        // Controller action logları için özel metotlar
        void LogRequest(HttpContext context, string controllerName, string actionName, object requestData = null);
        void LogResponse(HttpContext context, string controllerName, string actionName, object responseData = null, int statusCode = 200);
        void LogException(HttpContext context, string controllerName, string actionName, Exception ex);
    }
} 