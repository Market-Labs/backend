namespace marketgo_backend.Suppliers.Interfaces.Rest.Resources;
public sealed record UpdateSupplierResource(string? BusinessName, string? Ruc, string? Email, string? Phone,
    string? Address, string? Specialty, string? CoverageArea, string? Status);
