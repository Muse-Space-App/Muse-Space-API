using MuseSpace.BLL.DTO;
using MuseSpace.BLL.Interfaces.Services;

namespace MuseSpace.BLL.Services;

public sealed class SampleService : IService<SampleDto>
{
    public Task<IReadOnlyCollection<SampleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<SampleDto> data = new[]
        {
            new SampleDto { Id = 1, Name = "Template Item" }
        };

        return Task.FromResult(data);
    }
}
