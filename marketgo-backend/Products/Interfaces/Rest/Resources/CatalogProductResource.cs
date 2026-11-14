namespace marketgo_backend.Products.Interfaces.Rest.Resources;

public sealed record CatalogProductResource(Guid Id, Guid OrganizationId, string Name,
    string Description, string Category, decimal Price, bool Available);
