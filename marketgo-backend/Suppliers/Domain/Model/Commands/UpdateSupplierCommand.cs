namespace marketgo_backend.Suppliers.Domain.Model.Commands;
public sealed record UpdateSupplierCommand(Guid Id, Guid OrganizationId, string? BusinessName, string? Ruc,
    string? Email, string? Phone, string? Address, string? Specialty, string? CoverageArea, string? Status);
