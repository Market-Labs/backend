namespace marketgo_backend.Inventory.Domain.Model.Commands;
public sealed record CreateWasteCommand(Guid OrganizationId, Guid InventoryItemId, int Quantity, string Reason);
