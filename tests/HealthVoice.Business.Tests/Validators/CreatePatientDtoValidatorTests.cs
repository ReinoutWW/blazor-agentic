using FluentAssertions;
using FluentValidation;
using HealthVoice.Business.DTOs;
using HealthVoice.Business.Validators;

namespace HealthVoice.Business.Tests.Validators;

/// <summary>
/// Unit tests for CreatePatientDtoValidator following HealthVoice testing strategy.
/// Tests focus on validation rules without external dependencies.
/// </summary>
public class CreatePatientDtoValidatorTests
{
    private readonly CreatePatientDtoValidator _validator = new();

    [Fact]
    public async Task WHEN_AllFieldsValid_SHOULD_PassValidation()
    {
        // Arrange
        var dto = new CreatePatientDto(
            "John",
            "Doe",
            "john.doe@example.com",
            new DateOnly(1990, 5, 15)
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task WHEN_FirstNameIsEmpty_SHOULD_FailValidation(string? firstName)
    {
        // Arrange
        var dto = new CreatePatientDto(
            firstName!,
            "Doe",
            "john.doe@example.com",
            new DateOnly(1990, 5, 15)
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.FirstName) && 
                                           e.ErrorMessage == "First name is required");
    }

    [Fact]
    public async Task WHEN_FirstNameTooLong_SHOULD_FailValidation()
    {
        // Arrange
        var dto = new CreatePatientDto(
            new string('A', 101), // 101 characters
            "Doe",
            "john.doe@example.com",
            new DateOnly(1990, 5, 15)
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.FirstName) && 
                                           e.ErrorMessage == "First name cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("John123")]
    [InlineData("John@")]
    [InlineData("John#")]
    public async Task WHEN_FirstNameContainsInvalidCharacters_SHOULD_FailValidation(string firstName)
    {
        // Arrange
        var dto = new CreatePatientDto(
            firstName,
            "Doe",
            "john.doe@example.com",
            new DateOnly(1990, 5, 15)
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.FirstName) && 
                                           e.ErrorMessage == "First name can only contain letters, spaces, hyphens, and periods");
    }

    [Theory]
    [InlineData("John")]
    [InlineData("Mary-Jane")]
    [InlineData("Dr. Robert")]
    [InlineData("Anne Marie")]
    public async Task WHEN_FirstNameHasValidCharacters_SHOULD_PassValidation(string firstName)
    {
        // Arrange
        var dto = new CreatePatientDto(
            firstName,
            "Doe",
            "john.doe@example.com",
            new DateOnly(1990, 5, 15)
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(dto.FirstName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task WHEN_LastNameIsEmpty_SHOULD_FailValidation(string? lastName)
    {
        // Arrange
        var dto = new CreatePatientDto(
            "John",
            lastName!,
            "john.doe@example.com",
            new DateOnly(1990, 5, 15)
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.LastName) && 
                                           e.ErrorMessage == "Last name is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("john.doe@")]
    public async Task WHEN_EmailIsInvalid_SHOULD_FailValidation(string? email)
    {
        // Arrange
        var dto = new CreatePatientDto(
            "John",
            "Doe",
            email!,
            new DateOnly(1990, 5, 15)
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.Email));
    }

    [Fact]
    public async Task WHEN_EmailTooLong_SHOULD_FailValidation()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com"; // 262 characters total (250 + 12)
        var dto = new CreatePatientDto(
            "John",
            "Doe",
            longEmail,
            new DateOnly(1990, 5, 15)
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.Email) && 
                                           e.ErrorMessage == "Email cannot exceed 255 characters");
    }

    [Fact]
    public async Task WHEN_DateOfBirthInFuture_SHOULD_FailValidation()
    {
        // Arrange
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var dto = new CreatePatientDto(
            "John",
            "Doe",
            "john.doe@example.com",
            futureDate
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.DateOfBirth) && 
                                           e.ErrorMessage == "Date of birth must be in the past and not more than 150 years ago");
    }

    [Fact]
    public async Task WHEN_DateOfBirthTooOld_SHOULD_FailValidation()
    {
        // Arrange
        var tooOldDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-151));
        var dto = new CreatePatientDto(
            "John",
            "Doe",
            "john.doe@example.com",
            tooOldDate
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.DateOfBirth) && 
                                           e.ErrorMessage == "Date of birth must be in the past and not more than 150 years ago");
    }

    [Fact]
    public async Task WHEN_DateOfBirthIsValidRange_SHOULD_PassValidation()
    {
        // Arrange
        var validDate = new DateOnly(1990, 5, 15);
        var dto = new CreatePatientDto(
            "John",
            "Doe",
            "john.doe@example.com",
            validDate
        );

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(dto.DateOfBirth));
    }
} 