using marketgo_backend.Products.Domain.Model.Aggregates;
namespace marketgo_backend.Products.Interfaces.Acl;
public interface IProductsContextFacade
{
    Task<Product?> FindAvailableAsync(Guid productId, CancellationToken cancellationToken);
}
