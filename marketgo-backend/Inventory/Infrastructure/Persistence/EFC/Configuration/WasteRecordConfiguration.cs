using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace marketgo_backend.Inventory.Infrastructure.Persistence.EFC.Configuration;

public sealed class WasteRecordConfiguration : IEntityTypeConfiguration<WasteRecord>
{
    public void Configure(EntityTypeBuilder<WasteRecord> builder)
    {
        builder.ToTable("waste_records"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(160);
        builder.Property(x => x.LotCode).IsRequired().HasMaxLength(80);
        builder.Property(x => x.Unit).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(500);
        builder.HasIndex(x => new { x.OrganizationId, x.RecordedAt });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<InventoryItem>().WithMany().HasForeignKey(x => x.InventoryItemId).OnDelete(DeleteBehavior.Restrict);
    }
}
