using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using deneme.Models.Logs;
using deneme.Services.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace deneme.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogRepository _logRepository;
        private readonly IApiLogService _apiLogService;
        private readonly List<string> _excludedPaths;
        
        public RequestResponseLoggingMiddleware(
            RequestDelegate next, 
            ILogRepository logRepository,
            IApiLogService apiLogService,
            IConfiguration configuration)
        {
            _next = next;
            _logRepository = logRepository;
            _apiLogService = apiLogService;
            
            // Hariç tutulan endpoint'ler
            _excludedPaths = configuration.GetSection("LogSettings:ExcludedPaths")
                .Get<List<string>>() ?? new List<string>();
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            // Loglama yapılacak mı kontrol et
            if (ShouldSkipLogging(context.Request.Path))
            {
                await _next(context);
                return;
            }
            
            // Profil fotoğrafı endpoint'i ise özel işleme
            bool isProfilePictureEndpoint = context.Request.Path.ToString().Contains("/profile-picture", StringComparison.OrdinalIgnoreCase);
            
            var watch = Stopwatch.StartNew();
            var requestBody = "";
            
            // Profil fotoğrafı endpoint'i değilse request body'i oku
            if (!isProfilePictureEndpoint)
            {
                requestBody = await ReadRequestBodyAsync(context.Request);
            }
            
            // Orijinal response body stream'i kaydet
            var originalBodyStream = context.Response.Body;
            
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            
            try
            {
                // Sonraki middleware'i çalıştır
                await _next(context);
                
                // Response'u yakala
                string responseBodyText = "";
                
                // Profil fotoğrafı yanıtı değilse ve başarılı yanıtsa response body'i oku
                if (!isProfilePictureEndpoint || context.Response.StatusCode != 200)
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
                    responseBody.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    // Profil fotoğrafı endpoint'i için özet bilgi
                    responseBodyText = "[PROFILE PICTURE BINARY DATA - NOT LOGGED]";
                }
                
                // Log oluştur
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var username = context.User.FindFirstValue(ClaimTypes.Name);
                
                var log = new RequestResponseLog
                {
                    TraceId = traceId,
                    Path = context.Request.Path,
                    HttpMethod = context.Request.Method,
                    QueryString = context.Request.QueryString.ToString(),
                    RequestBody = isProfilePictureEndpoint ? "" : SanitizeBody(requestBody),
                    ResponseBody = SanitizeBody(responseBodyText),
                    StatusCode = context.Response.StatusCode,
                    UserId = userId,
                    Username = username,
                    UserIp = GetIpAddress(context),
                    RequestSize = context.Request.ContentLength ?? 0,
                    ResponseSize = context.Response.ContentLength ?? 0,
                    ExecutionTime = watch.ElapsedMilliseconds,
                    Timestamp = DateTime.UtcNow
                };
                
                // Log kaydet
                await _logRepository.SaveRequestResponseLogAsync(log);
            }
            catch (Exception ex)
            {
                // Exception durumunda API log kaydet
                await _apiLogService.LogErrorAsync(
                    "Middleware exception occurred", 
                    ex, 
                    context);
                
                // Eğer response gönderilmediyse bir hata response'u oluştur
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    
                    var result = JsonConvert.SerializeObject(new 
                    { 
                        statusCode = 500,
                        isSuccess = false,
                        message = "An unexpected error occurred"
                    });
                    
                    responseBody.SetLength(0);
                    await responseBody.WriteAsync(Encoding.UTF8.GetBytes(result));
                    responseBody.Seek(0, SeekOrigin.Begin);
                }
            }
            finally
            {
                // Response body'yi orijinal stream'e kopyala
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }
        
        private bool ShouldSkipLogging(PathString path)
        {
            return _excludedPaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
        }
        
        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var body = await reader.ReadToEndAsync();
            
            // Pozisyonu başa al
            request.Body.Position = 0;
            
            return body;
        }
        
        private string SanitizeBody(string body)
        {
            // Hassas verileri reddet (password, token vs.)
            var sensitiveFields = new[] { "password", "token", "refreshToken", "newPassword", 
                                           "confirmPassword", "confirmNewPassword", "currentPassword" };
            
            try
            {
                if (string.IsNullOrEmpty(body)) return string.Empty;
                
                // Binary içerik kontrolü (profil fotoğrafı vb.)
                if (IsBinaryContent(body))
                {
                    return "[BINARY DATA - NOT LOGGED]";
                }
                
                // JSON mı kontrol et
                if (!body.TrimStart().StartsWith("{") && !body.TrimStart().StartsWith("[")) 
                    return body;
                
                var jToken = JToken.Parse(body);
                
                if (jToken is JObject jObject)
                {
                    SanitizeJsonObject(jObject, sensitiveFields);
                    RemoveProfilePictureContent(jObject);
                }
                else if (jToken is JArray jArray)
                {
                    foreach (var item in jArray.OfType<JObject>())
                    {
                        SanitizeJsonObject(item, sensitiveFields);
                        RemoveProfilePictureContent(item);
                    }
                }
                
                // Formatlanmış JSON string'i al
                var serialized = jToken.ToString(Formatting.Indented);
                
                // String içindeki tüm JSON escape karakterlerini temizle
                serialized = serialized.Replace("\\r\\n", Environment.NewLine)
                                      .Replace("\\n", Environment.NewLine)
                                      .Replace("\\\"", "\"")
                                      .Replace("\\\\", "\\");
                
                return serialized;
            }
            catch
            {
                // JSON parse hatası durumunda - içerik binary veya geçersiz olabilir
                if (IsBinaryContent(body))
                {
                    return "[BINARY DATA - NOT LOGGED]";
                }
                
                // JSON parse hatası durumunda orijinal body'yi dön
                return body;
            }
        }
        
        private bool IsBinaryContent(string content)
        {
            if (string.IsNullOrEmpty(content)) return false;
            
            // Profil fotoğrafı URL'leri için kontrol
            if (content.Contains("/profile-picture", StringComparison.OrdinalIgnoreCase))
                return true;
            
            // JPEG başlık işaretleri kontrolü
            if (content.Contains("JFIF") || content.StartsWith("\u00FF\u00D8"))
                return true;
            
            // PNG başlık işaretleri kontrolü
            if (content.Contains("PNG") && content.Contains("IHDR"))
                return true;
            
            // PDF başlık işaretleri
            if (content.Contains("%PDF-"))
                return true;
            
            // ZIP, tar gibi arşiv formatları
            if (content.Contains("PK\u0003\u0004"))
                return true;
            
            // Binary karakter kontrolü
            // ASCII olmayan karakterlerin oranını kontrol et
            int nonAsciiChars = content.Count(c => c > 127 || c < 32 && c != '\r' && c != '\n' && c != '\t');
            return (double)nonAsciiChars / content.Length > 0.1; // %10'dan fazla binary karakter varsa
        }
        
        private void SanitizeJsonObject(JObject jObject, string[] sensitiveFields)
        {
            foreach (var property in jObject.Properties().ToList())
            {
                if (sensitiveFields.Any(field => property.Name.Contains(field, StringComparison.OrdinalIgnoreCase)))
                {
                    property.Value = "***REDACTED***";
                }
                else if (property.Value is JObject childObject)
                {
                    SanitizeJsonObject(childObject, sensitiveFields);
                }
                else if (property.Value is JArray childArray)
                {
                    foreach (var item in childArray.OfType<JObject>())
                    {
                        SanitizeJsonObject(item, sensitiveFields);
                    }
                }
            }
        }
        
        private void RemoveProfilePictureContent(JObject jObject)
        {
            // Profil fotoğrafı içeriğini temizle - genişletilmiş path kontrolü
            var pathsToCheck = new[]
            {
                "profilePicture.picture",
                "data.profilePicture.picture",
                "profilePicture",
                "picture",
                "image",
                "photo"
            };
            
            foreach (var path in pathsToCheck)
            {
                JToken token = jObject.SelectToken(path);
                if (token != null && token.Type == JTokenType.String)
                {
                    var stringValue = token.Value<string>();
                    if (!string.IsNullOrEmpty(stringValue) && IsBinaryContent(stringValue))
                    {
                        token.Replace("[PROFILE PICTURE DATA REMOVED]");
                    }
                }
            }
            
            // data içindeki profil fotoğraflarını kontrol et
            JToken dataToken = jObject.SelectToken("data");
            if (dataToken != null)
            {
                if (dataToken is JArray dataArray)
                {
                    foreach (var item in dataArray.OfType<JObject>())
                    {
                        foreach (var path in pathsToCheck)
                        {
                            JToken picToken = item.SelectToken(path);
                            if (picToken != null && picToken.Type == JTokenType.String)
                            {
                                var stringValue = picToken.Value<string>();
                                if (!string.IsNullOrEmpty(stringValue) && IsBinaryContent(stringValue))
                                {
                                    picToken.Replace("[PROFILE PICTURE DATA REMOVED]");
                                }
                            }
                        }
                    }
                }
                else if (dataToken is JObject dataObject)
                {
                    foreach (var path in pathsToCheck)
                    {
                        JToken picToken = dataObject.SelectToken(path);
                        if (picToken != null && picToken.Type == JTokenType.String)
                        {
                            var stringValue = picToken.Value<string>();
                            if (!string.IsNullOrEmpty(stringValue) && IsBinaryContent(stringValue))
                            {
                                picToken.Replace("[PROFILE PICTURE DATA REMOVED]");
                            }
                        }
                    }
                }
            }
        }
        
        private string GetIpAddress(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
} 