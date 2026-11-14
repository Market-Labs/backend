namespace marketgo_backend.Suppliers.Interfaces.Rest.Resources;
public sealed record SupplierResource(Guid Id, Guid? SupplierOrganizationId, string BusinessName, string Ruc, string Email, string Phone,
    string Address, string Specialty, string CoverageArea, string Status);
