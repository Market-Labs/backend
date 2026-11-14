namespace marketgo_backend.Inventory.Interfaces.Rest.Resources;
public sealed record CreateDonationResource(Guid InventoryItemId, int Quantity, string Recipient);
