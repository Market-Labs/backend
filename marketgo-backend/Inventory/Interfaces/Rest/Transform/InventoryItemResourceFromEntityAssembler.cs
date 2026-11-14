using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Interfaces.Rest.Resources;
namespace marketgo_backend.Inventory.Interfaces.Rest.Transform;
public static class InventoryItemResourceFromEntityAssembler
{
    public static InventoryItemResource ToResourceFromEntity(InventoryItem item) =>
        new(item.Id, item.ProductId, item.ProductName, item.Stock, item.MinimumStock,
            item.LotCode, item.ExpirationDate, item.Status);
}
