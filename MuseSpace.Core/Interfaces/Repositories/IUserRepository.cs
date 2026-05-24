namespace MuseSpace.Core.Interfaces.Repositories;

using MuseSpace.Core.Entities;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<User>> SearchAsync(string query, int skip, int take, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    new Task AddAsync(User user, CancellationToken cancellationToken = default);

    new Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
