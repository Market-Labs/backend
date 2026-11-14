using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Products.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Products.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class ProductRepository(AppDbContext context) : OrganizationRepository<Product>(context), IProductRepository
{
    public async Task<IReadOnlyList<Product>> ListAvailableAsync(Guid? organizationId, CancellationToken ct)
    {
        var query = Context.Set<Product>().AsNoTracking().Where(x => x.Available);
        if (organizationId.HasValue) query = query.Where(x => x.OrganizationId == organizationId.Value);
        return await query.OrderBy(x => x.Name).ToListAsync(ct);
    }
}
