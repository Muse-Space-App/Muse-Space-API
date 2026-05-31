using MuseSpace.Core.Entities;

namespace MuseSpace.Core.Interfaces.Repositories;

public interface IOtpRepository : IRepository<Otp>
{
    Task<Otp?> GetLatestValidOtpAsync(int userId, string purpose, CancellationToken cancellationToken = default);

    Task InvalidateUserOtpsAsync(int userId, string purpose, CancellationToken cancellationToken = default);
}
