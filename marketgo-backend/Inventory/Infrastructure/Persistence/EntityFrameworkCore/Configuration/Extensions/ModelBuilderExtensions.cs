using marketgo_backend.Inventory.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
public static class ModelBuilderExtensions
{
    public static void ApplyInventoryConfiguration(this ModelBuilder builder)
    {
        var item = builder.Entity<InventoryItem>();
        builder.Entity<InventoryItem>().Ignore(x => x.DomainEvents);
        item.ToTable("inventory_items"); item.HasKey(x => x.Id);
        item.HasIndex(x => new { x.OrganizationId, x.LotCode }).IsUnique();
        item.Property(x => x.ProductName).HasMaxLength(160).IsRequired();
        item.Property(x => x.LotCode).HasMaxLength(80).IsRequired();
        item.Ignore(x => x.Status);
        var waste = builder.Entity<WasteRecord>();
        waste.ToTable("waste_records"); waste.HasKey(x => x.Id);
        waste.HasIndex(x => x.OrganizationId);
        waste.Property(x => x.ProductName).HasMaxLength(160).IsRequired();
        waste.Property(x => x.LotCode).HasMaxLength(80).IsRequired();
        waste.Property(x => x.Reason).HasMaxLength(500).IsRequired();
        var donation = builder.Entity<DonationRecord>();
        donation.ToTable("donation_records"); donation.HasKey(x => x.Id);
        donation.HasIndex(x => x.OrganizationId);
        donation.Property(x => x.ProductName).HasMaxLength(160).IsRequired();
        donation.Property(x => x.LotCode).HasMaxLength(80).IsRequired();
        donation.Property(x => x.Recipient).HasMaxLength(160).IsRequired();
    }
}
