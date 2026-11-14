using marketgo_backend.Procurements.Domain.Model.ValueObjects;
namespace marketgo_backend.Procurements.Interfaces.Rest.Resources;
public sealed record ReviewPurchaseOrderResource(string Status, string? RejectionReason, IReadOnlyList<ReceivedOrderLine>? ReceivedLines = null);
