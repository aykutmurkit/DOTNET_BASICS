using FluentValidation;
using test.DTOs;

namespace test.Validators.NonGeneric.StationValidators
{
    public class UpdateStationDtoValidator : BaseValidator<UpdateStationDto>
    {
        public UpdateStationDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
                .Must(name => !name.Contains("NonGeneric")).WithMessage("Name cannot contain 'NonGeneric' for non-generic architecture");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required")
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0")
                .LessThan(1000).WithMessage("Capacity must be less than 1000 for non-generic architecture");
        }
    }
} 