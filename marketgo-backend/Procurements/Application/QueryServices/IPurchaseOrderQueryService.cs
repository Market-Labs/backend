using marketgo_backend.Procurements.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Domain.Model.Queries;
namespace marketgo_backend.Procurements.Application.QueryServices;
public interface IPurchaseOrderQueryService
{
    Task<IReadOnlyList<PurchaseOrder>> Handle(GetPurchaseOrdersQuery query, CancellationToken cancellationToken);
    Task<PurchaseOrder?> FindVisibleAsync(Guid id, Guid organizationId, string accountType, CancellationToken cancellationToken);
}
