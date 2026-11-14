using marketgo_backend.Procurements.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Interfaces.Rest.Resources;
namespace marketgo_backend.Procurements.Interfaces.Rest.Transform;
public static class PurchaseOrderResourceFromEntityAssembler
{
    public static PurchaseOrderResource ToResourceFromEntity(PurchaseOrder order) =>
        new(order.Id, order.SupplyRequestId, order.SupplierId, order.MinimarketId, order.Supplier,
            order.Minimarket, order.Status, order.Total, order.CreatedAt, order.ShippingDate,
            order.ReceivedAt, order.Observations, order.RejectionReason, order.Items);
}
