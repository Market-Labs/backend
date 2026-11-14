using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Suppliers.Domain.Model.Aggregates;
namespace marketgo_backend.Suppliers.Domain.Repositories;
public interface ISupplierRepository : IOrganizationRepository<SupplierProfile>
{
    Task<bool> ExistsRucAsync(Guid organizationId, string ruc, Guid? exceptId, CancellationToken cancellationToken);
}
