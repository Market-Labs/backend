using marketgo_backend.Communication.Interfaces.Acl;
using marketgo_backend.Dashboard.Application.QueryServices;
using marketgo_backend.Dashboard.Domain.Model.Queries;
using marketgo_backend.Dashboard.Domain.Model.ValueObjects;
using marketgo_backend.Inventory.Interfaces.Acl;
using marketgo_backend.Procurements.Interfaces.Acl;
using marketgo_backend.Requisition.Interfaces.Acl;
namespace marketgo_backend.Dashboard.Application.Internal.QueryServices;
public sealed class DashboardQueryService(IInventoryContextFacade inventory,
    IRequisitionContextFacade requisitions, IProcurementsContextFacade procurements,
    ICommunicationContextFacade communication) : IDashboardQueryService
{
    public async Task<DashboardSnapshot> Handle(GetDashboardQuery query, CancellationToken ct)
    {
        var stock = await inventory.GetMetricsAsync(query.OrganizationId, ct);
        var requests = await requisitions.CountForAsync(query.OrganizationId, query.AccountType, ct);
        var orders = await procurements.GetMetricsAsync(query.OrganizationId, query.AccountType, ct);
        var unread = await communication.CountUnreadAlertsAsync(query.OrganizationId, ct);
        var score = stock.ItemCount == 0 ? 100 :
            (int)Math.Round(100m * (stock.ItemCount - stock.RiskCount) / stock.ItemCount);
        DashboardIndicator[] indicators =
        [
            new("Stock disponible", "inventory", stock.Stock),
            new("Requisiciones", "requisition", requests),
            new("Abastecimientos", "procurements", orders.TotalOrders),
            new("Alertas activas", "alerts", unread, Severity: unread > 0 ? "warning" : "normal")
        ];
        return new DashboardSnapshot(score, indicators);
    }
}
