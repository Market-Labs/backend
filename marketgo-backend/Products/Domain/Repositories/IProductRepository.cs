using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Products.Domain.Repositories;
public interface IProductRepository : IOrganizationRepository<Product>
{
    Task<IReadOnlyList<Product>> ListAvailableAsync(Guid? organizationId, CancellationToken cancellationToken);
}
