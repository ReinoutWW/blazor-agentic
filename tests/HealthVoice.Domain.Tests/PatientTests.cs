using FluentAssertions;
using HealthVoice.Domain.Entities;

namespace HealthVoice.Domain.Tests;

/// <summary>
/// Unit tests for Patient entity following HealthVoice testing strategy.
/// Tests focus on domain invariants and behavior without external dependencies.
/// </summary>
public class PatientTests
{
    [Fact]
    public void WHEN_CreatingPatient_SHOULD_SetAllPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var dateOfBirth = new DateOnly(1990, 5, 15);
        var createdAt = DateTime.UtcNow;

        // Act
        var patient = new Patient
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = dateOfBirth,
            CreatedAt = createdAt
        };

        // Assert
        patient.Id.Should().Be(id);
        patient.FirstName.Should().Be(firstName);
        patient.LastName.Should().Be(lastName);
        patient.Email.Should().Be(email);
        patient.DateOfBirth.Should().Be(dateOfBirth);
        patient.CreatedAt.Should().Be(createdAt);
    }

    [Theory]
    [InlineData("John", "Doe", "John Doe")]
    [InlineData("Jane", "Smith", "Jane Smith")]
    [InlineData("Dr. Robert", "Johnson", "Dr. Robert Johnson")]
    public void WHEN_GettingFullName_SHOULD_CombineFirstAndLastName(string firstName, string lastName, string expectedFullName)
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Act
        var fullName = patient.FullName;

        // Assert
        fullName.Should().Be(expectedFullName);
    }

    [Theory]
    [InlineData(1990, 5, 15, 34)] // Fixed calculation for 2024-06-24
    [InlineData(2000, 1, 1, 24)]
    [InlineData(1985, 12, 31, 38)]
    public void WHEN_CalculatingAge_SHOULD_ReturnCorrectAge(int year, int month, int day, int expectedAge)
    {
        // Arrange
        var dateOfBirth = new DateOnly(year, month, day);
        var patient = new Patient { DateOfBirth = dateOfBirth };
        var currentDate = new DateOnly(2024, 6, 24); // Fixed date for testing

        // Act
        var age = CalculateAge(patient.DateOfBirth, currentDate);

        // Assert
        age.Should().Be(expectedAge);
    }

    [Fact]
    public void WHEN_PatientIsMinor_SHOULD_IdentifyCorrectly()
    {
        // Arrange
        var minorDateOfBirth = new DateOnly(2010, 1, 1); // 14 years old
        var adultDateOfBirth = new DateOnly(1990, 1, 1); // 34 years old
        var currentDate = new DateOnly(2024, 6, 24);

        var minorPatient = new Patient { DateOfBirth = minorDateOfBirth };
        var adultPatient = new Patient { DateOfBirth = adultDateOfBirth };

        // Act
        var minorAge = CalculateAge(minorPatient.DateOfBirth, currentDate);
        var adultAge = CalculateAge(adultPatient.DateOfBirth, currentDate);

        // Assert
        (minorAge < 18).Should().BeTrue("Patient should be identified as minor");
        (adultAge >= 18).Should().BeTrue("Patient should be identified as adult");
    }

    [Theory]
    [InlineData("john.doe@example.com")]
    [InlineData("jane.smith@hospital.org")]
    [InlineData("patient123@clinic.net")]
    public void WHEN_PatientHasValidEmail_SHOULD_AcceptEmail(string email)
    {
        // Arrange & Act
        var patient = new Patient { Email = email };

        // Assert
        patient.Email.Should().Be(email);
        patient.Email.Should().Contain("@");
        patient.Email.Should().Contain(".");
    }

    [Fact]
    public void WHEN_CreatingPatientWithDefaults_SHOULD_HaveValidState()
    {
        // Arrange & Act
        var patient = new Patient();

        // Assert
        patient.Id.Should().NotBe(Guid.Empty); // Auto-generated Guid
        patient.FirstName.Should().Be(string.Empty); // Default value from entity
        patient.LastName.Should().Be(string.Empty); // Default value from entity
        patient.Email.Should().Be(string.Empty); // Default value from entity
        patient.DateOfBirth.Should().Be(default(DateOnly));
        patient.CreatedAt.Should().Be(default(DateTime));
    }

    [Fact]
    public void WHEN_TwoPatientsHaveSameData_SHOULD_BeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var dateOfBirth = new DateOnly(1990, 5, 15);
        var createdAt = DateTime.UtcNow;

        var patient1 = new Patient
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = dateOfBirth,
            CreatedAt = createdAt
        };

        var patient2 = new Patient
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DateOfBirth = dateOfBirth,
            CreatedAt = createdAt
        };

        // Act & Assert
        patient1.Id.Should().Be(patient2.Id);
        patient1.FirstName.Should().Be(patient2.FirstName);
        patient1.LastName.Should().Be(patient2.LastName);
        patient1.Email.Should().Be(patient2.Email);
        patient1.DateOfBirth.Should().Be(patient2.DateOfBirth);
        patient1.CreatedAt.Should().Be(patient2.CreatedAt);
    }

    /// <summary>
    /// Helper method to calculate age - in a real scenario this might be a method on the Patient entity
    /// </summary>
    private static int CalculateAge(DateOnly dateOfBirth, DateOnly currentDate)
    {
        var age = currentDate.Year - dateOfBirth.Year;
        if (currentDate.DayOfYear < dateOfBirth.DayOfYear)
        {
            age--;
        }
        return age;
    }
}