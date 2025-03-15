using FluentValidation;

namespace test.Validators.NonGeneric
{
    /// <summary>
    /// Base validator for non-generic architecture
    /// </summary>
    /// <typeparam name="T">The type to validate</typeparam>
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        protected BaseValidator()
        {
            // Common validation rules can be added here
        }
    }
} 