using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Conservation.Domain.Repositories;
public interface IConservationRepository : IOrganizationRepository<ConservationReading>
{
    Task<IReadOnlyList<ConservationReading>> ListRecentAsync(Guid organizationId, CancellationToken cancellationToken);
}
