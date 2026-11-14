using marketgo_backend.Products.Application.QueryServices;
using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Products.Domain.Model.Queries;
using marketgo_backend.Products.Domain.Repositories;
namespace marketgo_backend.Products.Application.Internal.QueryServices;
public sealed class ProductQueryService(IProductRepository products) : IProductQueryService
{
    public Task<IReadOnlyList<Product>> Handle(GetProductsQuery query, CancellationToken ct) =>
        products.ListAvailableAsync(query.OrganizationId, ct);
    public async Task<Product?> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await products.FindByIdAsync(query.Id, ct);
        return product is { Available: true } ? product : null;
    }
}
