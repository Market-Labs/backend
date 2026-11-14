namespace marketgo_backend.Inventory.Domain.Model.Commands;
public sealed record CreateInventoryItemCommand(Guid OrganizationId, Guid ProductId, int Stock,
    int MinimumStock, string LotCode, DateOnly ExpirationDate);
