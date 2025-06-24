using FluentValidation;
using HealthVoice.Business.DTOs;

namespace HealthVoice.Business.Validators;

/// <summary>
/// Validator for CreatePatientDto following FluentValidation patterns
/// </summary>
public sealed class CreatePatientDtoValidator : AbstractValidator<CreatePatientDto>
{
    public CreatePatientDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s\-\.]+$").WithMessage("First name can only contain letters, spaces, hyphens, and periods");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s\-\.]+$").WithMessage("Last name can only contain letters, spaces, hyphens, and periods");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(BeValidDateOfBirth).WithMessage("Date of birth must be in the past and not more than 150 years ago");
    }

    private static bool BeValidDateOfBirth(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var minDate = today.AddYears(-150);
        
        return dateOfBirth >= minDate && dateOfBirth < today;
    }
} 