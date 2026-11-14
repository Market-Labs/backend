namespace marketgo_backend.Inventory.Interfaces.Rest.Resources;
public sealed record InventoryItemResource(Guid Id, Guid ProductId, string ProductName, int Stock,
    int MinimumStock, string LotCode, DateOnly ExpirationDate, string Status);
