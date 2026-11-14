namespace marketgo_backend.Inventory.Interfaces.Rest.Resources;
public sealed record CreateWasteResource(Guid InventoryItemId, int Quantity, string Reason);
