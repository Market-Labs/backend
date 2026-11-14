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
        item.Property(x => x.ExpirationDate)
            .HasConversion(value => value.ToDateTime(TimeOnly.MinValue),
                value => DateOnly.FromDateTime(value)).HasColumnType("date");
        item.Ignore(x => x.Status);
        var receipt = builder.Entity<InventoryReceiptMovement>();
        receipt.ToTable("inventory_receipt_movements"); receipt.HasKey(x => x.Id);
        receipt.HasIndex(x => new { x.PurchaseOrderId, x.LineIndex, x.LotCode }).IsUnique().HasDatabaseName("ux_receipt_order_line_lot");
        receipt.HasIndex(x => new { x.OrganizationId, x.InventoryItemId }).HasDatabaseName("ix_receipt_org_inventory");
        receipt.Property(x => x.LotCode).HasMaxLength(80).IsRequired();
        receipt.Property(x => x.ExpirationDate)
            .HasConversion(value => value.ToDateTime(TimeOnly.MinValue),
                value => DateOnly.FromDateTime(value)).HasColumnType("date");
        receipt.Ignore(x => x.DomainEvents);
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
