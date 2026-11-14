using marketgo_backend.Procurements.Domain.Model.ValueObjects;
namespace marketgo_backend.Procurements.Domain.Model.Commands;
public sealed record ReviewPurchaseOrderCommand(Guid Id, Guid MinimarketId, int ReviewerId,
    string Status, string? RejectionReason, IReadOnlyList<ReceivedOrderLine>? ReceivedLines);
