namespace marketgo_backend.Inventory.Domain.Model.Commands;
public sealed record UpdateInventoryItemCommand(Guid Id, Guid OrganizationId, int? Stock, int? MinimumStock);
