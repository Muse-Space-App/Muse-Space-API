namespace MuseSpace.Core.Interfaces.Services;

public interface IService<T>
{
    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
