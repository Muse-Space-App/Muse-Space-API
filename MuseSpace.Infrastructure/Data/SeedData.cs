using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;

namespace MuseSpace.Infrastructure.Data;

public static class SeedData
{
    /// <summary>
    /// Default roles that should be seeded into the database
    /// </summary>
    public static IReadOnlyCollection<string> DefaultRoles => new[]
    {
        "Admin",
        "Member"
    };

    /// <summary>
    /// Seeds default roles (idempotent - won't duplicate)
    /// ACID-compliant: Uses explicit transaction
    /// </summary>
    public static async Task SeedRolesAsync(this MuseSpaceDbContext context)
    {
        var existingRoles = await context.Roles.ToListAsync();
        var rolesToAdd = new List<Role>();

        foreach (var roleName in DefaultRoles)
        {
            if (!existingRoles.Any(r => r.Name.ToUpper() == roleName.ToUpper()))
            {
                rolesToAdd.Add(new Role
                {
                    Name = roleName,
                    Description = GetRoleDescription(roleName),
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = "System.SeedData"
                });
            }
        }

        if (rolesToAdd.Any())
        {
            await context.Roles.AddRangeAsync(rolesToAdd);
            Console.WriteLine($"Seeded {rolesToAdd.Count} role(s)");
        }
        else
        {
            Console.WriteLine("All roles already exist in the database");
        }
    }

    private static string GetRoleDescription(string roleName) => roleName switch
    {
        "Admin" => "Administrator",
        "Member" => "Regular user",
        _ => $"User with {roleName} role"
    };
}

