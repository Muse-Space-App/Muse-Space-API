using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

public class TagRepository : Repository<Tag>, ITagRepository
{
    private readonly MuseSpaceDbContext _dbContext;

    public TagRepository(MuseSpaceDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Tag>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<Tag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Tag>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Tag>> GetByArtworkIdAsync(int artworkId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Tag>()
            .AsNoTracking()
            .Where(t => t.ArtworkTags.Any(at => at.ArtworkId == artworkId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Tag>> GetPopularAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Tag>()
            .AsNoTracking()
            .Where(t => !t.IsBanned)
            .OrderByDescending(t => t.UsageCount)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Tag>> SearchAsync(string query, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tags
            .AsNoTracking()
            .Where(t => t.Name.Contains(query) || t.Slug.Contains(query))
            .OrderByDescending(t => t.UsageCount)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tag> GetOrCreateAsync(string name, string createdBy, CancellationToken cancellationToken = default)
    {
        var tag = await GetByNameAsync(name, cancellationToken);
        if (tag != null)
        {
            return tag;
        }

        var slug = GenerateSlug(name);

        // Ensure slug is unique
        var baseSlug = slug;
        int counter = 1;
        while (await GetBySlugAsync(slug, cancellationToken) != null)
        {
            slug = $"{baseSlug}-{counter++}";
        }

        tag = new Tag
        {
            Name = name,
            Slug = slug,
            UsageCount = 0,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _dbContext.Set<Tag>().AddAsync(tag, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return tag;
    }

    public async Task IncrementUsageCountAsync(int tagId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.ExecuteSqlRawAsync(
            "UPDATE \"Tags\" SET \"UsageCount\" = \"UsageCount\" + 1 WHERE \"Id\" = {0}",
            tagId);
    }

    private static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        // Very simple slug generation
        var slug = text.ToLowerInvariant();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-").Trim('-');

        return slug;
    }
}
