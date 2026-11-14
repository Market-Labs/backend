namespace marketgo_backend.Requisition.Domain.Model.Commands;
public sealed record ReviewSupplyRequestCommand(Guid Id, Guid SupplierId, string Status, string? RejectionReason);
