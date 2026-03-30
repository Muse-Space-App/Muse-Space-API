namespace MuseSpace.Core.Interfaces.Repositories;

public interface IRepository<T>
{
    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
}
