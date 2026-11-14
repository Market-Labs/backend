namespace marketgo_backend.Products.Interfaces.Rest.Resources;
public sealed record CreateProductResource(string Name, string Category, string Description,
    DateOnly ExpirationDate, int Quantity, decimal Price, Guid? SupplierId);
