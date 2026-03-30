namespace MuseSpace.Infrastructure.Data;

public static class SeedData
{
    public static IReadOnlyCollection<string> DefaultRoles => new[]
    {
        "Admin",
        "Member"
    };
}
