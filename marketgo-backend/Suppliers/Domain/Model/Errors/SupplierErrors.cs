using marketgo_backend.Shared.Domain.Model.Errors;
namespace marketgo_backend.Suppliers.Domain.Model.Errors;
public static class SupplierErrors
{
    public static readonly Error InvalidData = Error.Validation("Suppliers.InvalidData", "Invalid supplier data.");
    public static readonly Error RucAlreadyExists = Error.Conflict("Suppliers.RucAlreadyExists", "RUC is already registered for this organization.");
    public static readonly Error SupplierNotFound = Error.NotFound("Suppliers.SupplierNotFound", "Supplier not found.");
}
