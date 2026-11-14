using marketgo_backend.Products.Domain.Model.Aggregates;
namespace marketgo_backend.Products.Interfaces.Acl;
public interface IProductsContextFacade
{
    Task<Product?> FindAvailableAsync(Guid productId, CancellationToken cancellationToken);
    Task<Product> ResolveForReceiptAsync(Guid minimarketId, Guid supplierId, string productName,
        Guid? productId, string? category, string? description, DateOnly expirationDate,
        decimal unitPrice, CancellationToken cancellationToken);
}
