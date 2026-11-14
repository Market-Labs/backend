namespace marketgo_backend.Inventory.Domain.Model.Commands;
public sealed record CreateDonationCommand(Guid OrganizationId, Guid InventoryItemId, int Quantity, string Recipient);
