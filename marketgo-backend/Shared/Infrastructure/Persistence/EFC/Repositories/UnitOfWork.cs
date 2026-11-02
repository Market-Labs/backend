using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace marketgo_backend.Shared.Infrastructure.Persistence.EFC.Repositories;

/// <summary>Persists the changes tracked by the scoped EF Core context.</summary>
public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
