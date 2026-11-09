using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace marketgo_backend.Inventory.Infrastructure.Persistence.EFC.Configuration;

public sealed class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(160);
        builder.Property(x => x.LotCode).IsRequired().HasMaxLength(80);
        builder.Ignore(x => x.Status);
        builder.HasIndex(x => new { x.OrganizationId, x.ProductId });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
