namespace MuseSpace.BLL.Interfaces.Services;

public interface IAiDescriptionService
{
    Task<string?> GenerateDescriptionAsync(string imageUrl, CancellationToken cancellationToken = default);
}
