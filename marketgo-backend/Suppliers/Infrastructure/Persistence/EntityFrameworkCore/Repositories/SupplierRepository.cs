using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using marketgo_backend.Suppliers.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Suppliers.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class SupplierRepository(AppDbContext context)
    : OrganizationRepository<SupplierProfile>(context), ISupplierRepository
{
    public Task<bool> ExistsRucAsync(Guid organizationId, string ruc, Guid? exceptId, CancellationToken ct) =>
        Context.Set<SupplierProfile>().AnyAsync(s => s.OrganizationId == organizationId && s.Ruc == ruc &&
            (!exceptId.HasValue || s.Id != exceptId.Value), ct);
}
