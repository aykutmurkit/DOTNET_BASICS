using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DeviceApi.TCPListener.Services;

namespace DeviceApi.TCPListener.Extensions
{
    /// <summary>
    /// IApplicationBuilder için eklenti metotlarını içeren sınıf
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// TCP Listener servisini kullanmak için gerekli yapılandırmaları yapar
        /// </summary>
        /// <param name="app">Application builder nesnesi</param>
        /// <returns>Application builder nesnesi</returns>
        public static IApplicationBuilder UseTcpListener(this IApplicationBuilder app)
        {
            // Bu metot şu anda doğrudan bir işlem yapmıyor çünkü TCP Listener
            // bir Hosted Service olarak zaten otomatik başlatılıyor.
            // Ancak ileride middleware gibi ek işlemler gerekirse buraya eklenebilir.
            
            return app;
        }
    }
} 