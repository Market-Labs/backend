using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Products.Domain.Repositories;
using marketgo_backend.Products.Interfaces.Acl;
namespace marketgo_backend.Products.Application.Acl;
public sealed class ProductsContextFacade(IProductRepository products) : IProductsContextFacade
{
    public async Task<Product> ResolveForReceiptAsync(Guid minimarketId, Guid supplierId,
        string productName, Guid? productId, string? category, string? description,
        DateOnly expirationDate, decimal unitPrice, CancellationToken ct)
    {
        if (productId.HasValue)
        {
            var selected = await products.FindByIdAsync(productId.Value, ct);
            if (selected is null || !selected.Available ||
                selected.OrganizationId != minimarketId && selected.OrganizationId != supplierId ||
                !string.Equals(selected.Name, productName, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Product reference is not valid for this order.");
            return selected;
        }
        var own = (await products.ListAvailableAsync(minimarketId, ct))
            .Where(x => string.Equals(x.Name, productName, StringComparison.OrdinalIgnoreCase)).ToList();
        if (own.Count > 1) throw new ArgumentException("Product selection is ambiguous.");
        if (own.Count == 1) return own[0];
        var supplier = (await products.ListAvailableAsync(supplierId, ct))
            .Where(x => string.Equals(x.Name, productName, StringComparison.OrdinalIgnoreCase)).ToList();
        if (supplier.Count > 1) throw new ArgumentException("Product selection is ambiguous.");
        if (supplier.Count == 1) return supplier[0];
        if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A new product requires category and description.");
        var created = new Product(minimarketId, productName, category, description,
            expirationDate, 0, unitPrice, null);
        await products.AddAsync(created, ct);
        return created;
    }
    public async Task<Product?> FindAvailableAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await products.FindByIdAsync(productId, cancellationToken);
        return product is { Available: true } ? product : null;
    }
}
