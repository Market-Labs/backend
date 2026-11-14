using marketgo_backend.Conservation.Application.QueryServices;
using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Conservation.Domain.Model.Queries;
using marketgo_backend.Conservation.Domain.Repositories;
namespace marketgo_backend.Conservation.Application.Internal.QueryServices;
public sealed class ConservationQueryService(IConservationRepository readings) : IConservationQueryService
{
    public Task<IReadOnlyList<ConservationReading>> Handle(GetConservationReadingsQuery query, CancellationToken ct) =>
        readings.ListRecentAsync(query.OrganizationId, ct);
}
