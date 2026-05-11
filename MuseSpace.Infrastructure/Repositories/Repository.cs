using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for all entities
/// Provides basic CRUD operations and follows ACID principles
/// </summary>
public sealed class Repository<T> : IRepository<T> where T : class
{
    private readonly MuseSpaceDbContext _dbContext;

    /// <summary>
    /// Constructor with dependency injection of the DbContext
    /// </summary>
    /// <param name="dbContext"></param>
    public Repository(MuseSpaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves all entities of type T from the database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    public async Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves an entity by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new entity to the database
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing entity in the database
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes an entity by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (entity != null)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Adds multiple entities in a single transaction
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates multiple entities in a single transaction
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().UpdateRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
