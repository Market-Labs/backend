using marketgo_backend.Suppliers.Domain.Repositories;
using marketgo_backend.Suppliers.Interfaces.Acl;
namespace marketgo_backend.Suppliers.Application.Acl;
public sealed class SuppliersContextFacade(ISupplierRepository repository) : ISuppliersContextFacade
{
    public async Task<bool> ExistsActiveAsync(Guid supplierId, CancellationToken ct) =>
        await repository.FindByIdAsync(supplierId, ct) is { Status: "active" };
    public async Task<int> CountActiveAsync(Guid id, CancellationToken ct) =>
        (await repository.ListByOrganizationAsync(id, ct)).Count(x => x.Status == "active");
}
