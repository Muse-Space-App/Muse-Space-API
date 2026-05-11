using MuseSpace.Core.Interfaces.Repositories;

namespace MuseSpace.Infrastructure.Repositories;

public sealed class InMemoryRepository<T> : IRepository<T>
{
    private readonly IReadOnlyCollection<T> _items;

    public InMemoryRepository(IReadOnlyCollection<T> items)
    {
        _items = items;
    }

    public Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_items);
    }

    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<T?>(default);
    }

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("In-memory repository is read-only.");
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("In-memory repository is read-only.");
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("In-memory repository is read-only.");
    }

    public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("In-memory repository is read-only.");
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("In-memory repository is read-only.");
    }
}
