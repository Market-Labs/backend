using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class OrganizationRepository<TEntity>(AppDbContext context) : IOrganizationRepository<TEntity>
    where TEntity : class
{
    protected readonly AppDbContext Context = context;

    public async Task<IReadOnlyList<TEntity>> ListByOrganizationAsync(Guid organizationId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<TEntity>().AsNoTracking()
            .Where(entity => EF.Property<Guid>(entity, "OrganizationId") == organizationId)
            .ToListAsync(cancellationToken);

    public Task<TEntity?> FindByIdForOrganizationAsync(Guid id, Guid organizationId,
        CancellationToken cancellationToken = default) =>
        Context.Set<TEntity>().FirstOrDefaultAsync(entity =>
            EF.Property<Guid>(entity, "Id") == id &&
            EF.Property<Guid>(entity, "OrganizationId") == organizationId, cancellationToken);

    public Task<TEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Set<TEntity>().FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == id, cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);

    public void Remove(TEntity entity) => Context.Set<TEntity>().Remove(entity);
}
