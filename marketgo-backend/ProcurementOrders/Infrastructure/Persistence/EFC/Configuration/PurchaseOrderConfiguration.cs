using marketgo_backend.Organizations.Domain.Model.Aggregates;
using marketgo_backend.ProcurementOrders.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace marketgo_backend.ProcurementOrders.Infrastructure.Persistence.EFC.Configuration;

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Observations).HasMaxLength(1000);
        builder.Property(x => x.RejectionReason).HasMaxLength(1000);
        builder.Property(x => x.Status).HasMaxLength(24).IsRequired();
        builder.Property(x => x.Total).HasPrecision(12, 2);
        builder.Property(x => x.ItemsJson).HasColumnType("longtext").IsRequired();
        builder.Ignore(x => x.Items);
        builder.HasIndex(x => x.MinimarketId);
        builder.HasIndex(x => x.SupplierId);
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.MinimarketId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<SupplierProfile>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<SupplyRequest>().WithOne().HasForeignKey<PurchaseOrder>(x => x.SupplyRequestId).OnDelete(DeleteBehavior.Restrict);
    }
}
