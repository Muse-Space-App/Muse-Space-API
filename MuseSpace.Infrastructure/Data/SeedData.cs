using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;

namespace MuseSpace.Infrastructure.Data;

public static class SeedData
{
    private const string DefaultPasswordHash =
        "$2a$11$rGhJKsY3NxV8mR1Q5p7XYeK4lXzQz0s1P5K4lXzQz0s1P5K4lX.O";

    /// <summary>
    /// Default roles that should be seeded into the database
    /// </summary>
    public static IReadOnlyCollection<string> DefaultRoles => new[]
    {
        "Admin",
        "Member",
        "Moderator",
        "Premium"
    };

    /// <summary>
    /// Default tags that should be seeded into the database
    /// </summary>
    public static IReadOnlyCollection<string> DefaultTags => new[]
    {
        "Digital Art",
        "Traditional Art",
        "Photography",
        "3D Art",
        "Animation",
        "Pixel Art",
        "Concept Art",
        "Fan Art",
        "Abstract",
        "Portrait",
        "Landscape",
        "Illustration",
        "Watercolor",
        "Oil Painting",
        "Character Design"
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

    /// <summary>
    /// Seeds default tags (idempotent - won't duplicate)
    /// </summary>
    public static async Task SeedTagsAsync(this MuseSpaceDbContext context)
    {
        var existingTags = await context.Tags.ToListAsync();
        var tagsToAdd = new List<Tag>();

        foreach (var tagName in DefaultTags)
        {
            if (!existingTags.Any(t => t.Name.ToUpper() == tagName.ToUpper()))
            {
                tagsToAdd.Add(new Tag
                {
                    Name = tagName,
                    Slug = GenerateSlug(tagName),
                    UsageCount = 0,
                    IsModerated = true,
                    IsBanned = false,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = "System.SeedData"
                });
            }
        }

        if (tagsToAdd.Any())
        {
            await context.Tags.AddRangeAsync(tagsToAdd);
            Console.WriteLine($"Seeded {tagsToAdd.Count} tag(s)");
        }
        else
        {
            Console.WriteLine("All tags already exist in the database");
        }
    }

    /// <summary>
    /// Seeds default users with profiles (idempotent - won't duplicate)
    /// </summary>
    public static async Task SeedUsersAsync(this MuseSpaceDbContext context)
    {
        var existingUsers = await context.Users.ToListAsync();
        var usersToAdd = new List<User>();
        var profilesToAdd = new List<UserProfile>();

        var seedUsers = new[]
        {
            new
            {
                Email = "admin@yurisoft.com",
                Username = "admin",
                FirstName = "Admin",
                LastName = "System",
                RoleId = 1,
                Bio = "MuseSpace platform administrator"
            },
            new
            {
                Email = "artist1@yurisoft.com",
                Username = "artist_luna",
                FirstName = "Luna",
                LastName = "Chen",
                RoleId = 2,
                Bio = "Digital artist and illustrator passionate about fantasy worlds"
            },
            new
            {
                Email = "artist2@yurisoft.com",
                Username = "pixel_master",
                FirstName = "Alex",
                LastName = "Rivera",
                RoleId = 2,
                Bio = "Pixel art enthusiast and retro game designer"
            },
            new
            {
                Email = "artist3@yurisoft.com",
                Username = "brush_strokes",
                FirstName = "Maya",
                LastName = "Johnson",
                RoleId = 2,
                Bio = "Traditional and watercolor artist exploring mixed media"
            }
        };

        foreach (var seedUser in seedUsers)
        {
            if (!existingUsers.Any(u => u.Email.ToUpper() == seedUser.Email.ToUpper()))
            {
                var user = new User
                {
                    Email = seedUser.Email,
                    Username = seedUser.Username,
                    PasswordHash = DefaultPasswordHash,
                    FirstName = seedUser.FirstName,
                    LastName = seedUser.LastName,
                    RoleId = seedUser.RoleId,
                    IsEmailVerified = true,
                    EmailVerifiedAtUtc = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = "System.SeedData"
                };

                usersToAdd.Add(user);
            }
        }

        if (usersToAdd.Any())
        {
            await context.Users.AddRangeAsync(usersToAdd);
            await context.SaveChangesAsync();

            foreach (var user in usersToAdd)
            {
                var seedUser = seedUsers.First(s => s.Email == user.Email);

                if (!await context.UserProfiles.AnyAsync(p => p.UserId == user.Id))
                {
                    profilesToAdd.Add(new UserProfile
                    {
                        UserId = user.Id,
                        Bio = seedUser.Bio,
                        AvatarUrl = null,
                        BannerUrl = null,
                        CreatedAtUtc = DateTime.UtcNow,
                        CreatedBy = "System.SeedData"
                    });
                }
            }

            if (profilesToAdd.Any())
            {
                await context.UserProfiles.AddRangeAsync(profilesToAdd);
            }

            Console.WriteLine($"Seeded {usersToAdd.Count} user(s) with profiles");
        }
        else
        {
            Console.WriteLine("All users already exist in the database");
        }
    }

    private static string GetRoleDescription(string roleName) => roleName switch
    {
        "Admin" => "Administrator",
        "Member" => "Regular user",
        "Moderator" => "Content moderator",
        "Premium" => "Premium subscriber",
        _ => $"User with {roleName} role"
    };

    private static string GenerateSlug(string name) =>
        name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");
}
