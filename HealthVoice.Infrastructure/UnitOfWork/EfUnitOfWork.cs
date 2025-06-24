using HealthVoice.Domain.Contracts;
using HealthVoice.Infrastructure.Data;
using HealthVoice.Infrastructure.Repositories;

namespace HealthVoice.Infrastructure.UnitOfWork;

/// <summary>
/// Entity Framework Unit of Work implementation
/// </summary>
public class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed;

    public EfUnitOfWork(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IRepository<T> Repo<T>() where T : class
    {
        var type = typeof(T);

        if (_repositories.TryGetValue(type, out var existingRepo))
        {
            return (IRepository<T>)existingRepo;
        }

        var repository = new EfRepository<T>(_context);
        _repositories[type] = repository;
        return repository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _repositories.Clear();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}