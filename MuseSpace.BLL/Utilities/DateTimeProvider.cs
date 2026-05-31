using MuseSpace.BLL.Interfaces.Services;

namespace MuseSpace.BLL.Utilities;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
