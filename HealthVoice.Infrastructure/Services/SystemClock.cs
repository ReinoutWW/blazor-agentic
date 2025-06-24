using HealthVoice.Domain.Contracts;

namespace HealthVoice.Infrastructure.Services;

/// <summary>
/// System clock implementation
/// </summary>
public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime Now => DateTime.Now;

    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}