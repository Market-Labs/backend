namespace marketgo_backend.Products.Interfaces.Rest.Resources;
public sealed record UpdateProductResource(string? Name, string? Category, string? Description,
    DateOnly? ExpirationDate, int? Quantity, decimal? Price, bool? Available, Guid? SupplierId);
