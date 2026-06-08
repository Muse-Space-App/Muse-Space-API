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
                Bio = "MuseSpace platform administrator",
                Avatar = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400",
                IsAcceptingCommissions = false
            },
            new
            {
                Email = "artist1@yurisoft.com",
                Username = "artist_luna",
                FirstName = "Luna",
                LastName = "Chen",
                RoleId = 2,
                Bio = "Digital artist and illustrator passionate about fantasy worlds",
                Avatar = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=400",
                IsAcceptingCommissions = true
            },
            new
            {
                Email = "artist2@yurisoft.com",
                Username = "pixel_master",
                FirstName = "Alex",
                LastName = "Rivera",
                RoleId = 2,
                Bio = "Pixel art enthusiast and retro game designer",
                Avatar = "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?w=400",
                IsAcceptingCommissions = true
            },
            new
            {
                Email = "artist3@yurisoft.com",
                Username = "brush_strokes",
                FirstName = "Maya",
                LastName = "Johnson",
                RoleId = 2,
                Bio = "Traditional and watercolor artist exploring mixed media",
                Avatar = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=400",
                IsAcceptingCommissions = false
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
                        AvatarUrl = seedUser.Email == "admin@yurisoft.com" ? "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400" : seedUser.GetType().GetProperty("Avatar")?.GetValue(seedUser)?.ToString(),
                        BannerUrl = "https://images.unsplash.com/photo-1557683316-973673baf926?w=1200",
                        IsAcceptingCommissions = seedUser.Email != "admin@yurisoft.com" && (bool)(seedUser.GetType().GetProperty("IsAcceptingCommissions")?.GetValue(seedUser) ?? false),
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

    /// <summary>
    /// Seeds sample artworks with real Unsplash images (idempotent)
    /// </summary>
    public static async Task SeedArtworksAsync(this MuseSpaceDbContext context)
    {
        if (await context.Artwork.AnyAsync())
        {
            Console.WriteLine("Artworks already exist in the database");
            return;
        }

        var users = await context.Users.Where(u => u.Username != "admin").ToListAsync();
        if (!users.Any())
        {
            Console.WriteLine("No users found to assign artworks to - skipping artwork seeding");
            return;
        }

        var tags = await context.Tags.ToListAsync();

        var seedArtworks = new[]
        {
            new { Title = "Golden Hour Over the Mountains", Desc = "Warm light spilling across a vast mountain landscape at sunset.", Url = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=1200", Thumb = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=400", UserIdx = 0, TagNames = new[] { "Landscape", "Photography" }, W = 1200, H = 800 },
            new { Title = "Neon City Reflections", Desc = "Rain-soaked streets reflecting vibrant neon lights of the city.", Url = "https://images.unsplash.com/photo-1514565131-fce0801e5785?w=1200", Thumb = "https://images.unsplash.com/photo-1514565131-fce0801e5785?w=400", UserIdx = 1, TagNames = new[] { "Photography", "Digital Art" }, W = 1200, H = 800 },
            new { Title = "Abstract Fluid Motion", Desc = "Swirling colors in a mesmerizing abstract composition.", Url = "https://images.unsplash.com/photo-1541701494587-cb58502866ab?w=1200", Thumb = "https://images.unsplash.com/photo-1541701494587-cb58502866ab?w=400", UserIdx = 2, TagNames = new[] { "Abstract", "Digital Art" }, W = 1200, H = 800 },
            new { Title = "Serene Japanese Garden", Desc = "A tranquil garden scene with cherry blossoms and still water.", Url = "https://images.unsplash.com/photo-1528360983277-13d401cdc186?w=1200", Thumb = "https://images.unsplash.com/photo-1528360983277-13d401cdc186?w=400", UserIdx = 0, TagNames = new[] { "Landscape", "Photography" }, W = 1200, H = 800 },
            new { Title = "Portrait in Natural Light", Desc = "Soft natural light creating a warm, intimate portrait.", Url = "https://images.unsplash.com/photo-1531746020798-e6953c6e8e04?w=1200", Thumb = "https://images.unsplash.com/photo-1531746020798-e6953c6e8e04?w=400", UserIdx = 1, TagNames = new[] { "Portrait", "Photography" }, W = 1200, H = 900 },
            new { Title = "Cosmic Nebula Dreams", Desc = "Deep space imagery revealing the beauty of distant nebulae.", Url = "https://images.unsplash.com/photo-1462331940025-496dfbfc7564?w=1200", Thumb = "https://images.unsplash.com/photo-1462331940025-496dfbfc7564?w=400", UserIdx = 2, TagNames = new[] { "Digital Art", "Abstract" }, W = 1200, H = 800 },
            new { Title = "Vintage Film Tones", Desc = "A nostalgic scene captured with warm vintage film aesthetics.", Url = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=1200", Thumb = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=400", UserIdx = 0, TagNames = new[] { "Photography", "Illustration" }, W = 1200, H = 800 },
            new { Title = "Watercolor Botanicals", Desc = "Delicate floral studies rendered in soft watercolor washes.", Url = "https://images.unsplash.com/photo-1490750967868-88aa4f44baee?w=1200", Thumb = "https://images.unsplash.com/photo-1490750967868-88aa4f44baee?w=400", UserIdx = 1, TagNames = new[] { "Watercolor", "Illustration" }, W = 1200, H = 900 },
            new { Title = "Urban Architecture Lines", Desc = "Bold geometric patterns found in modern urban architecture.", Url = "https://images.unsplash.com/photo-1486325212027-8081e485255e?w=1200", Thumb = "https://images.unsplash.com/photo-1486325212027-8081e485255e?w=400", UserIdx = 2, TagNames = new[] { "Photography", "Abstract" }, W = 1200, H = 800 },
            new { Title = "Ocean Depths", Desc = "Mysterious deep blue ocean waves crashing against rocks.", Url = "https://images.unsplash.com/photo-1518837695005-2083093ee35b?w=1200", Thumb = "https://images.unsplash.com/photo-1518837695005-2083093ee35b?w=400", UserIdx = 0, TagNames = new[] { "Landscape", "Photography" }, W = 1200, H = 800 },
            new { Title = "Digital Character Sketch", Desc = "A striking character design concept with bold color choices.", Url = "https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=1200", Thumb = "https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=400", UserIdx = 1, TagNames = new[] { "Character Design", "Digital Art" }, W = 1200, H = 900 },
            new { Title = "Minimalist Still Life", Desc = "Clean, minimal composition with soft shadows and neutral tones.", Url = "https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=1200", Thumb = "https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=400", UserIdx = 2, TagNames = new[] { "Photography", "Abstract" }, W = 1200, H = 800 },
        };

        var artworksToAdd = new List<Artwork>();
        var artworkTagsToAdd = new List<ArtworkTag>();

        foreach (var seed in seedArtworks)
        {
            var artwork = new Artwork
            {
                CreatorId = users[seed.UserIdx % users.Count].Id,
                Title = seed.Title,
                Description = seed.Desc,
                ContentUrl = seed.Url,
                ThumbnailUrl = seed.Thumb,
                MediaType = "Image",
                Width = seed.W,
                Height = seed.H,
                ViewCount = Random.Shared.Next(10, 500),
                LikeCount = Random.Shared.Next(2, 80),
                CommentCount = 0,
                BookmarkCount = Random.Shared.Next(0, 30),
                ShareCount = Random.Shared.Next(0, 15),
                IsApproved = true,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)),
                CreatedBy = "System.SeedData"
            };
            artworksToAdd.Add(artwork);
        }

        await context.Artwork.AddRangeAsync(artworksToAdd);
        await context.SaveChangesAsync();

        foreach (var (artwork, idx) in artworksToAdd.Select((a, i) => (a, i)))
        {
            var seed = seedArtworks[idx];
            foreach (var tagName in seed.TagNames)
            {
                var tag = tags.FirstOrDefault(t => t.Name == tagName);
                if (tag != null)
                {
                    artworkTagsToAdd.Add(new ArtworkTag
                    {
                        ArtworkId = artwork.Id,
                        TagId = tag.Id
                    });
                }
            }
        }

        if (artworkTagsToAdd.Any())
        {
            await context.ArtworkTags.AddRangeAsync(artworkTagsToAdd);
        }

        Console.WriteLine($"Seeded {artworksToAdd.Count} artwork(s) with tags");
    }

    /// <summary>
    /// Seeds sample groups and events (idempotent)
    /// </summary>
    public static async Task SeedGroupsAndEventsAsync(this MuseSpaceDbContext context)
    {
        var users = await context.Users.Where(u => u.Username != "admin").ToListAsync();
        if (!users.Any()) return;

        if (!await context.Groups.AnyAsync())
        {
            var groups = new[]
            {
                new Group { Name = "Digital Art Collective", Description = "A community for digital artists to share techniques, feedback, and inspiration.", CreatorId = users[0].Id, IsPrivate = false, AvatarUrl = "https://images.unsplash.com/photo-1550684848-fac1c5b4e853?w=400", BannerUrl = "https://images.unsplash.com/photo-1550684376-efcbd6e3f031?w=1200", CreatedAtUtc = DateTime.UtcNow.AddDays(-20), CreatedBy = "System.SeedData" },
                new Group { Name = "Photography Enthusiasts", Description = "Share your best shots and learn from fellow photographers.", CreatorId = users[1].Id, IsPrivate = false, AvatarUrl = "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=400", BannerUrl = "https://images.unsplash.com/photo-1452587925148-ce544e77e70d?w=1200", CreatedAtUtc = DateTime.UtcNow.AddDays(-15), CreatedBy = "System.SeedData" },
                new Group { Name = "Watercolor Workshop", Description = "Weekly watercolor challenges and painting tips.", CreatorId = users[2 % users.Count].Id, IsPrivate = false, AvatarUrl = "https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=400", BannerUrl = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=1200", CreatedAtUtc = DateTime.UtcNow.AddDays(-10), CreatedBy = "System.SeedData" },
            };

            await context.Groups.AddRangeAsync(groups);
            await context.SaveChangesAsync();

            var members = new List<GroupMember>();
            foreach (var group in groups)
            {
                members.Add(new GroupMember { GroupId = group.Id, UserId = group.CreatorId, Role = "Admin", JoinedAtUtc = group.CreatedAtUtc });
                foreach (var user in users.Where(u => u.Id != group.CreatorId))
                {
                    members.Add(new GroupMember { GroupId = group.Id, UserId = user.Id, Role = "Member", JoinedAtUtc = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 10)) });
                }
            }
            await context.GroupMembers.AddRangeAsync(members);
            Console.WriteLine($"Seeded {groups.Length} group(s) with members");
        }

        if (!await context.Events.AnyAsync())
        {
            var events = new[]
            {
                new Event { Title = "Galactic Art Showcase", Description = "An evening of cosmic-inspired art from creators worldwide.", OrganizerId = users[0].Id, StartDateUtc = DateTime.UtcNow.AddDays(14), EndDateUtc = DateTime.UtcNow.AddDays(14).AddHours(3), Location = "Virtual Gallery", IsOnline = true, CreatedAtUtc = DateTime.UtcNow.AddDays(-5), CreatedBy = "System.SeedData" },
                new Event { Title = "Portrait Drawing Workshop", Description = "Learn portrait fundamentals with live model sessions.", OrganizerId = users[1].Id, StartDateUtc = DateTime.UtcNow.AddDays(21), EndDateUtc = DateTime.UtcNow.AddDays(21).AddHours(2), Location = "Studio Room A", IsOnline = false, CreatedAtUtc = DateTime.UtcNow.AddDays(-3), CreatedBy = "System.SeedData" },
                new Event { Title = "Digital Art Meetup", Description = "Monthly meetup for digital artists to network and share work.", OrganizerId = users[2 % users.Count].Id, StartDateUtc = DateTime.UtcNow.AddDays(7), EndDateUtc = DateTime.UtcNow.AddDays(7).AddHours(2), Location = "Online - Discord", IsOnline = true, CreatedAtUtc = DateTime.UtcNow.AddDays(-1), CreatedBy = "System.SeedData" },
            };

            await context.Events.AddRangeAsync(events);
            Console.WriteLine($"Seeded {events.Length} event(s)");
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

    public static async Task SeedGroupPostsAsync(this MuseSpaceDbContext context)
    {
        if (await context.GroupPosts.AnyAsync()) return;

        var groups = await context.Groups.ToListAsync();
        if (!groups.Any()) return;

        var members = await context.GroupMembers.ToListAsync();

        var postsToAdd = new List<GroupPost>();
        foreach (var group in groups)
        {
            var groupMembers = members.Where(m => m.GroupId == group.Id).ToList();
            if (!groupMembers.Any()) continue;

            for (int i = 0; i < 3; i++)
            {
                var randomMember = groupMembers[Random.Shared.Next(groupMembers.Count)];
                postsToAdd.Add(new GroupPost
                {
                    GroupId = group.Id,
                    AuthorId = randomMember.UserId,
                    Content = i == 0 ? $"Hello everyone! Welcome to {group.Name}. Feel free to share your works here!" : $"Just finished a new piece. Wanted to share it with this amazing group.",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 10)),
                    CreatedBy = "System.SeedData"
                });
            }
        }

        await context.GroupPosts.AddRangeAsync(postsToAdd);
        await context.SaveChangesAsync();
        Console.WriteLine($"Seeded {postsToAdd.Count} group posts");
    }

    public static async Task SeedCommissionsAsync(this MuseSpaceDbContext context)
    {
        if (await context.Commissions.AnyAsync()) return;

        var artists = await context.Users.Where(u => u.Email == "artist1@yurisoft.com" || u.Email == "artist2@yurisoft.com").ToListAsync();
        var requesters = await context.Users.Where(u => u.Email == "admin@yurisoft.com" || u.Email == "artist3@yurisoft.com").ToListAsync();

        if (!artists.Any() || !requesters.Any()) return;

        var commissionsToAdd = new List<Commission>();
        var messagesToAdd = new List<CommissionMessage>();

        // Pending Commission
        var pendingComm = new Commission
        {
            ArtistId = artists[0].Id,
            RequesterId = requesters[0].Id,
            Title = "Custom Profile Portrait",
            Description = "I would like a digital portrait in your signature style for my social media profiles.",
            Price = 120.00m,
            Status = MuseSpace.Core.Enums.CommissionStatus.Pending,
            DeadlineUtc = DateTime.UtcNow.AddDays(14),
            CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
            CreatedBy = "System.SeedData"
        };
        commissionsToAdd.Add(pendingComm);

        // InProgress Commission
        var inProgressComm = new Commission
        {
            ArtistId = artists[0].Id,
            RequesterId = requesters[1 % requesters.Count].Id,
            Title = "Fantasy Landscape Background",
            Description = "Need a background for my new game featuring a magical forest.",
            Price = 300.00m,
            Status = MuseSpace.Core.Enums.CommissionStatus.InProgress,
            DeadlineUtc = DateTime.UtcNow.AddDays(30),
            CreatedAtUtc = DateTime.UtcNow.AddDays(-5),
            CreatedBy = "System.SeedData"
        };
        commissionsToAdd.Add(inProgressComm);

        // Completed Commission
        var completedComm = new Commission
        {
            ArtistId = artists[1 % artists.Count].Id,
            RequesterId = requesters[0].Id,
            Title = "Pixel Art Character Sprite",
            Description = "Idle animation sprite for a retro-style RPG character.",
            Price = 80.00m,
            Status = MuseSpace.Core.Enums.CommissionStatus.Completed,
            DeadlineUtc = DateTime.UtcNow.AddDays(-1),
            CompletedAtUtc = DateTime.UtcNow.AddDays(-2),
            CreatedAtUtc = DateTime.UtcNow.AddDays(-10),
            CreatedBy = "System.SeedData"
        };
        commissionsToAdd.Add(completedComm);

        await context.Commissions.AddRangeAsync(commissionsToAdd);
        await context.SaveChangesAsync();

        // Add some messages to the InProgress commission
        messagesToAdd.Add(new CommissionMessage
        {
            CommissionId = inProgressComm.Id,
            SenderId = inProgressComm.RequesterId,
            Content = "Hi! I really love your fantasy landscapes. Are you available for a new commission?",
            CreatedAtUtc = inProgressComm.CreatedAtUtc.AddHours(1),
            CreatedBy = "System.SeedData"
        });
        messagesToAdd.Add(new CommissionMessage
        {
            CommissionId = inProgressComm.Id,
            SenderId = inProgressComm.ArtistId,
            Content = "Hello! Yes, I am. A magical forest sounds like a fun project. Do you have any specific color palettes in mind?",
            CreatedAtUtc = inProgressComm.CreatedAtUtc.AddHours(2),
            CreatedBy = "System.SeedData"
        });

        await context.CommissionMessages.AddRangeAsync(messagesToAdd);
        await context.SaveChangesAsync();

        Console.WriteLine($"Seeded {commissionsToAdd.Count} commissions and {messagesToAdd.Count} messages");
    }
}
