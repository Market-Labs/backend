using marketgo_backend.Products.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Products.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
public static class ModelBuilderExtensions
{
    public static void ApplyProductsConfiguration(this ModelBuilder builder)
    {
        var product = builder.Entity<Product>();
        builder.Entity<Product>().Ignore(x => x.DomainEvents);
        product.ToTable("products"); product.HasKey(x => x.Id);
        product.HasIndex(x => x.OrganizationId);
        product.Property(x => x.Name).HasMaxLength(160).IsRequired();
        product.Property(x => x.Category).HasMaxLength(80).IsRequired();
        product.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        product.Property(x => x.Price).HasPrecision(12, 2);
    }
}
