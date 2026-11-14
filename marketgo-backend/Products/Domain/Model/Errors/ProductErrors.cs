using marketgo_backend.Shared.Domain.Model.Errors;
namespace marketgo_backend.Products.Domain.Model.Errors;
public static class ProductErrors
{
    public static readonly Error SupplierUnavailable = Error.Validation("Products.SupplierUnavailable", "Supplier is unavailable.");
    public static readonly Error ProductNotFound = Error.NotFound("Products.ProductNotFound", "Product not found.");
    public static readonly Error InvalidProductData = Error.Validation("Products.InvalidProductData", "Invalid product data.");
}
