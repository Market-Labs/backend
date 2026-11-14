namespace marketgo_backend.Products.Domain.Model.Commands;
public sealed record UpdateProductCommand(Guid Id, Guid OrganizationId, string? Name, string? Category,
    string? Description, DateOnly? ExpirationDate, int? Quantity, decimal? Price, bool? Available, Guid? SupplierId);
