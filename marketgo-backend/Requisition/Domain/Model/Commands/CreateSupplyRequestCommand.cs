using marketgo_backend.Requisition.Domain.Model.ValueObjects;
namespace marketgo_backend.Requisition.Domain.Model.Commands;
public sealed record CreateSupplyRequestCommand(Guid MinimarketId, Guid SupplierOrganizationId, int RequesterId,
    string Requester, string Supplier, string Reason, IReadOnlyList<SupplyRequestItem> Items);
