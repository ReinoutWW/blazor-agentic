namespace HealthVoice.Domain.Contracts;

/// <summary>
/// Abstraction for system clock operations
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current UTC date and time
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Gets the current local date and time
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Gets the current date only (local)
    /// </summary>
    DateOnly Today { get; }
}