using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Products.Domain.Repositories;
using marketgo_backend.Products.Interfaces.Acl;
namespace marketgo_backend.Products.Application.Acl;
public sealed class ProductsContextFacade(IProductRepository products) : IProductsContextFacade
{
    public async Task<Product?> FindAvailableAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await products.FindByIdAsync(productId, cancellationToken);
        return product is { Available: true } ? product : null;
    }
}
