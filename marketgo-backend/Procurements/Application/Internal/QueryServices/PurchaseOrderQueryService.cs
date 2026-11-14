using marketgo_backend.Procurements.Application.QueryServices;
using marketgo_backend.Procurements.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Domain.Model.Queries;
using marketgo_backend.Procurements.Domain.Repositories;
namespace marketgo_backend.Procurements.Application.Internal.QueryServices;
public sealed class PurchaseOrderQueryService(IPurchaseOrderRepository orders) : IPurchaseOrderQueryService
{
    public Task<IReadOnlyList<PurchaseOrder>> Handle(GetPurchaseOrdersQuery query, CancellationToken ct) =>
        orders.ListForAsync(query.OrganizationId, query.AccountType, ct);
    public async Task<PurchaseOrder?> FindVisibleAsync(Guid id, Guid organizationId, string type, CancellationToken ct)
    {
        var order = await orders.FindAsync(id, ct);
        return order is not null &&
            (type == "supplier" ? order.SupplierId == organizationId : order.MinimarketId == organizationId)
            ? order : null;
    }
}
