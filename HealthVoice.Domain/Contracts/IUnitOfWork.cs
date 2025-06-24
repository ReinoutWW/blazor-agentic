namespace HealthVoice.Domain.Contracts;

/// <summary>
/// Unit of Work interface for managing transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets a repository for the specified entity type
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <returns>Repository instance</returns>
    IRepository<T> Repo<T>() where T : class;

    /// <summary>
    /// Saves all changes made in this unit of work
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}