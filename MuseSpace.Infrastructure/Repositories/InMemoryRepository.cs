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
}
