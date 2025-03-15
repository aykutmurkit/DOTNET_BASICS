using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace test.Extensions
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Adds FluentValidation services for the generic architecture
        /// </summary>
        public static IServiceCollection AddGenericValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Validators.Generic.BaseValidator<object>>(ServiceLifetime.Scoped, 
                filter: scan => scan.ValidatorType.Namespace?.StartsWith("test.Validators.Generic") == true);
            
            return services;
        }

        /// <summary>
        /// Adds FluentValidation services for the non-generic architecture
        /// </summary>
        public static IServiceCollection AddNonGenericValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Validators.NonGeneric.BaseValidator<object>>(ServiceLifetime.Scoped, 
                filter: scan => scan.ValidatorType.Namespace?.StartsWith("test.Validators.NonGeneric") == true);
            
            return services;
        }
    }
} 