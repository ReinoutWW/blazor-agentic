namespace HealthVoice.Domain.Entities;

/// <summary>
/// Patient domain entity - immutable record following DDD principles
/// </summary>
public record Patient
{
    /// <summary>
    /// Gets the patient identifier
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the patient's first name
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the patient's last name
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the patient's email address
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Gets the patient's date of birth
    /// </summary>
    public DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Gets when the patient record was created
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the patient's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Calculates the patient's current age
    /// </summary>
    /// <param name="asOfDate">The date to calculate age as of (defaults to today)</param>
    /// <returns>The patient's age in years</returns>
    public int CalculateAge(DateOnly? asOfDate = null)
    {
        var currentDate = asOfDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var age = currentDate.Year - DateOfBirth.Year;
        
        if (currentDate.DayOfYear < DateOfBirth.DayOfYear)
        {
            age--;
        }
        
        return age;
    }

    /// <summary>
    /// Determines if the patient is a minor (under 18)
    /// </summary>
    /// <param name="asOfDate">The date to check minor status as of (defaults to today)</param>
    /// <returns>True if the patient is under 18, false otherwise</returns>
    public bool IsMinor(DateOnly? asOfDate = null) => CalculateAge(asOfDate) < 18;

    /// <summary>
    /// Creates a new Patient with updated information
    /// </summary>
    /// <param name="firstName">Updated first name</param>
    /// <param name="lastName">Updated last name</param>
    /// <param name="email">Updated email</param>
    /// <returns>New Patient instance with updated data</returns>
    public Patient UpdateContact(string firstName, string lastName, string email)
    {
        return this with 
        { 
            FirstName = firstName?.Trim() ?? string.Empty,
            LastName = lastName?.Trim() ?? string.Empty,
            Email = email?.Trim().ToLowerInvariant() ?? string.Empty
        };
    }
}