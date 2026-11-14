namespace marketgo_backend.Inventory.Interfaces.Rest.Resources;
public sealed record CreateInventoryItemResource(Guid ProductId, int Stock, int MinimumStock,
    string LotCode, DateOnly ExpirationDate);
