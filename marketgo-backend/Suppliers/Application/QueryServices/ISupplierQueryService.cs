using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using marketgo_backend.Suppliers.Domain.Model.Queries;
namespace marketgo_backend.Suppliers.Application.QueryServices;
public interface ISupplierQueryService
{
    Task<IReadOnlyList<SupplierProfile>> Handle(GetSuppliersQuery query, CancellationToken cancellationToken);
    Task<SupplierProfile?> Handle(GetSupplierByIdQuery query, CancellationToken cancellationToken);
}
