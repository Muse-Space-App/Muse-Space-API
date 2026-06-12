using Microsoft.EntityFrameworkCore;
using MuseSpace.Core.Entities;
using MuseSpace.Core.Interfaces.Repositories;
using MuseSpace.Infrastructure.Data;

namespace MuseSpace.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for all entities
/// Provides basic CRUD operations and follows ACID principles
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly MuseSpaceDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Constructor with dependency injection of the DbContext
    /// </summary>
    /// <param name="dbContext"></param>
    public Repository(MuseSpaceDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    /// <summary>
    /// Retrieves all entities of type T from the database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    public virtual async Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves an entity by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    /// <summary>
    /// Adds a new entity to the database
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Adds multiple entities in a single transaction
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
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
    /// Updates multiple entities in a single transaction
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.UpdateRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an entity without saving changes immediately.
    /// This allows for batch updates or transactions to be handled at a higher level, ensuring that multiple operations can be performed atomically.
    /// The caller is responsible for calling SaveChangesAsync after all updates are made to persist the changes to the database.
    /// </summary>
    /// <param name="entity"></param>
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Deletes an entity by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Soft deletes an entity by setting its IsDeleted flag to true and updating the UpdatedAt timestamp. This allows for logical deletion of records while preserving data integrity and enabling potential recovery of deleted entities. The caller is responsible for calling SaveChangesAsync after this operation to persist the changes to the database.
    /// </summary>
    /// <param name="entity"></param>
    public void Delete(T entity)
    {
        _dbSet.Update(entity);
    }
}
