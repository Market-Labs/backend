using marketgo_backend.Requisition.Domain.Model.ValueObjects;
namespace marketgo_backend.Requisition.Interfaces.Rest.Resources;
public sealed record CreateSupplyRequestResource(Guid SupplierId, string Supplier, string Reason,
    IReadOnlyList<SupplyRequestItem> Items);
