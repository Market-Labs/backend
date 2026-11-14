using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Conservation.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Conservation.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class ConservationRepository(AppDbContext context)
    : OrganizationRepository<ConservationReading>(context), IConservationRepository
{
    public async Task<IReadOnlyList<ConservationReading>> ListRecentAsync(Guid id, CancellationToken ct) =>
        await Context.Set<ConservationReading>().AsNoTracking().Where(x => x.OrganizationId == id)
            .OrderByDescending(x => x.RecordedAt).Take(500).ToListAsync(ct);
}
