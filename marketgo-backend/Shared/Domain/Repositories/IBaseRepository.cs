namespace marketgo_backend.Shared.Domain.Repositories;

/// <summary>Common persistence operations for an aggregate with a context-defined key.</summary>
public interface IBaseRepository<TEntity, in TKey> where TEntity : class
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
