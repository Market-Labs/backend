using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Products.Domain.Model.Queries;
namespace marketgo_backend.Products.Application.QueryServices;
public interface IProductQueryService
{
    Task<IReadOnlyList<Product>> Handle(GetProductsQuery query, CancellationToken cancellationToken);
    Task<Product?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken);
}
