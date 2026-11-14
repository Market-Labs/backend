using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Model.Queries;
namespace marketgo_backend.Requisition.Application.QueryServices;
public interface ISupplyRequestQueryService
{
    Task<IReadOnlyList<SupplyRequest>> Handle(GetSupplyRequestsQuery query, CancellationToken cancellationToken);
    Task<SupplyRequest?> FindVisibleAsync(Guid id, Guid organizationId, string accountType, CancellationToken cancellationToken);
}
