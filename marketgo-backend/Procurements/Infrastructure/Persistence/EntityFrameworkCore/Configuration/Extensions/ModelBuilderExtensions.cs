using marketgo_backend.Procurements.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Procurements.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
public static class ModelBuilderExtensions
{
    public static void ApplyProcurementsConfiguration(this ModelBuilder builder)
    {
        var order = builder.Entity<PurchaseOrder>();
        order.ToTable("purchase_orders"); order.HasKey(x => x.Id);
        order.HasIndex(x => x.SupplierId); order.HasIndex(x => x.MinimarketId);
        order.HasIndex(x => x.SupplyRequestId).IsUnique();
        order.Property(x => x.Supplier).HasMaxLength(160).IsRequired();
        order.Property(x => x.Minimarket).HasMaxLength(160).IsRequired();
        order.Property(x => x.Status).HasMaxLength(30).IsRequired();
        order.Property(x => x.Observations).HasMaxLength(1000);
        order.Property(x => x.RejectionReason).HasMaxLength(1000);
        order.Property(x => x.Total).HasPrecision(12, 2);
        order.Property(x => x.ItemsJson).HasColumnType("longtext");
        order.Ignore(x => x.Items); order.Ignore(x => x.DomainEvents);
    }
}
