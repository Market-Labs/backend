using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Conservation.Domain.Model.Queries;
namespace marketgo_backend.Conservation.Application.QueryServices;
public interface IConservationQueryService
{
    Task<IReadOnlyList<ConservationReading>> Handle(GetConservationReadingsQuery query, CancellationToken cancellationToken);
}
