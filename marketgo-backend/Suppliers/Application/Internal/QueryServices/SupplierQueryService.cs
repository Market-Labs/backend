using marketgo_backend.Suppliers.Application.QueryServices;
using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using marketgo_backend.Suppliers.Domain.Model.Queries;
using marketgo_backend.Suppliers.Domain.Repositories;
namespace marketgo_backend.Suppliers.Application.Internal.QueryServices;
public sealed class SupplierQueryService(ISupplierRepository suppliers) : ISupplierQueryService
{
    public Task<IReadOnlyList<SupplierProfile>> Handle(GetSuppliersQuery query, CancellationToken ct) =>
        suppliers.ListByOrganizationAsync(query.OrganizationId, ct);
    public Task<SupplierProfile?> Handle(GetSupplierByIdQuery query, CancellationToken ct) =>
        suppliers.FindByIdForOrganizationAsync(query.Id, query.OrganizationId, ct);
}
