using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using BCrypt.Net;

namespace MuseSpace.BLL.Services;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public InMemoryUserRepository()
    {
        // Seed with a test user for development
        _users.Add(new User
        {
            Id = _nextId++,
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123", workFactor: 12),
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
            IsEmailVerified = true,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "system"
        });
    }

    public Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<User>>(_users.AsReadOnly());
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        user.Id = _nextId++;
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            var index = _users.IndexOf(existingUser);
            _users[index] = user;
        }
        return Task.CompletedTask;
    }
}
