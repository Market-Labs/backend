using marketgo_backend.Shared.Domain.Model.Errors;
namespace marketgo_backend.Inventory.Domain.Model.Errors;
public static class InventoryErrors
{
    public static readonly Error ItemNotFound = Error.NotFound("Inventory.ItemNotFound", "Inventory item not found.");
    public static readonly Error ProductUnavailable = Error.Validation("Inventory.ProductUnavailable", "Product is unavailable.");
    public static readonly Error InvalidData = Error.Validation("Inventory.InvalidData", "Invalid inventory data or insufficient stock.");
}
