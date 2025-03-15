using FluentValidation;

namespace test.Validators.Generic
{
    /// <summary>
    /// Base validator for generic architecture
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