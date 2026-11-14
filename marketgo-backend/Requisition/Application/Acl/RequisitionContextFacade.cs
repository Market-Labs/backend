using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Repositories;
using marketgo_backend.Requisition.Interfaces.Acl;
namespace marketgo_backend.Requisition.Application.Acl;
public sealed class RequisitionContextFacade(ISupplyRequestRepository requests) : IRequisitionContextFacade
{
    public async Task<SupplyRequest?> FindAcceptedAsync(Guid id, Guid supplierId, CancellationToken ct)
    {
        var request = await requests.FindAsync(id, ct);
        return request is { Status: "accepted", ShippingOrderId: null } && request.SupplierId == supplierId ? request : null;
    }
    public async Task<int> CountForAsync(Guid organizationId, string accountType, CancellationToken ct) =>
        (await requests.ListForAsync(organizationId, accountType, ct)).Count;
    public void LinkOrder(SupplyRequest request, Guid orderId) => request.LinkOrder(orderId);
}
