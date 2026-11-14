namespace marketgo_backend.Products.Interfaces.Rest.Resources;
public sealed record ProductResource(Guid Id, string Name, string Description, string Category,
    DateOnly ExpirationDate, int Quantity, decimal Price, bool Available, Guid? SupplierId);
