using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using marketgo_backend.Suppliers.Interfaces.Rest.Resources;
namespace marketgo_backend.Suppliers.Interfaces.Rest.Transform;
public static class SupplierResourceFromEntityAssembler
{
    public static SupplierResource ToResourceFromEntity(SupplierProfile supplier) =>
        new(supplier.Id, supplier.BusinessName, supplier.Ruc, supplier.Email, supplier.Phone,
            supplier.Address, supplier.Specialty, supplier.CoverageArea, supplier.Status);
}
