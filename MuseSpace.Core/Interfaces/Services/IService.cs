namespace MuseSpace.Core.Interfaces.Services;

public interface IService<T>
{
    /// <summary>
    /// Gets all entities of type T. This method should return a read-only collection of all entities, and it should support cancellation through the provided CancellationToken. The implementation should ensure that the data retrieval is efficient and handles any potential exceptions that may occur during the operation.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
