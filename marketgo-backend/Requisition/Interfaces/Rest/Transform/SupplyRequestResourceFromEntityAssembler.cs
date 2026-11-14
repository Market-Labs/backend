using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Interfaces.Rest.Resources;
namespace marketgo_backend.Requisition.Interfaces.Rest.Transform;
public static class SupplyRequestResourceFromEntityAssembler
{
    public static SupplyRequestResource ToResourceFromEntity(SupplyRequest request)
    {
        var items = request.Items;
        return new(request.Id, request.MinimarketId, request.SupplierId, request.RequesterId,
            request.Requester, request.Supplier, items.FirstOrDefault()?.ProductName ?? string.Empty,
            items.Sum(x => x.Quantity), request.Reason, request.Status, request.CreatedAt,
            request.ReviewedAt, request.RejectionReason, request.ShippingOrderId, items);
    }
}
