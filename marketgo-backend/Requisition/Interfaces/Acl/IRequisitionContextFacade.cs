using marketgo_backend.Requisition.Domain.Model.Aggregates;
namespace marketgo_backend.Requisition.Interfaces.Acl;
public interface IRequisitionContextFacade
{
    Task<SupplyRequest?> FindAcceptedAsync(Guid requestId, Guid supplierId, CancellationToken cancellationToken);
    Task<int> CountForAsync(Guid organizationId, string accountType, CancellationToken cancellationToken);
    void LinkOrder(SupplyRequest request, Guid orderId);
}
