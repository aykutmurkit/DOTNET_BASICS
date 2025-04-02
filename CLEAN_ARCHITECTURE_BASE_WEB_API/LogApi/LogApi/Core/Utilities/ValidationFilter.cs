using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Core.Utilities
{
    /// <summary>
    /// Model validasyon hatalarını yakalayıp standart formatta dönüş sağlayan filtre
    /// </summary>
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Model validasyonu başarısız olursa
            if (!context.ModelState.IsValid)
            {
                // Hata sözlüğü oluşturma
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                // Standart hata yanıtı oluşturma
                var response = ApiResponse<object>.Error(errors);
                
                // Yanıtı HTTP 400 olarak dönme
                context.Result = new BadRequestObjectResult(response);
                return;
            }

            // Validasyon başarılıysa işleme devam et
            await next();
        }
    }
} 