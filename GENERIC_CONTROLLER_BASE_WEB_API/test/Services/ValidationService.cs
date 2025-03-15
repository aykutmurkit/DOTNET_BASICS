using FluentValidation;

namespace test.Services
{
    public interface IValidationService
    {
        /// <summary>
        /// Validates an object using the appropriate validator
        /// </summary>
        Task ValidateAsync<T>(T obj, bool isGeneric = true);
    }

    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ValidateAsync<T>(T obj, bool isGeneric = true)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // Get all validators for the type
            var validators = _serviceProvider.GetServices<IValidator<T>>().ToList();

            if (!validators.Any())
                return;

            // Filter validators based on namespace (generic or non-generic)
            var filteredValidators = validators
                .Where(v => isGeneric 
                    ? v.GetType().Namespace?.StartsWith("test.Validators.Generic") == true
                    : v.GetType().Namespace?.StartsWith("test.Validators.NonGeneric") == true)
                .ToList();

            if (!filteredValidators.Any())
                return;

            // Validate using all appropriate validators
            foreach (var validator in filteredValidators)
            {
                var validationResult = await validator.ValidateAsync(obj);
                
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
            }
        }
    }
} 