namespace HealthVoice.Domain.Entities;

/// <summary>
/// Patient domain entity
/// </summary>
public class Patient
{
    /// <summary>
    /// Gets the patient identifier
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the patient's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the patient's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the patient's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the patient's date of birth
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets when the patient record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the patient's full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}