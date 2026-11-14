using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Products.Interfaces.Rest.Resources;
namespace marketgo_backend.Products.Interfaces.Rest.Transform;
public static class ProductResourceFromEntityAssembler
{
    public static CatalogProductResource ToCatalogResourceFromEntity(Product product) =>
        new(product.Id, product.OrganizationId, product.Name, product.Description,
            product.Category, product.Price, product.Available);

    public static ProductResource ToResourceFromEntity(Product product) =>
        new(product.Id, product.Name, product.Description, product.Category, product.ExpirationDate,
            product.Quantity, product.Price, product.Available, product.SupplierId);
}
