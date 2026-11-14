namespace marketgo_backend.Products.Domain.Model.Commands;
public sealed record CreateProductCommand(Guid OrganizationId, string Name, string Category,
    string Description, DateOnly ExpirationDate, int Quantity, decimal Price, Guid? SupplierId);
