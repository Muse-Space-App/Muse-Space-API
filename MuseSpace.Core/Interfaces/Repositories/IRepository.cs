using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
