namespace marketgo_backend.Shared.Domain.Repositories;

public interface IOrganizationRepository<TEntity> where TEntity : class
{
    Task<IReadOnlyList<TEntity>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<TEntity?> FindByIdForOrganizationAsync(Guid id, Guid organizationId, CancellationToken cancellationToken = default);
    Task<TEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Remove(TEntity entity);
}
