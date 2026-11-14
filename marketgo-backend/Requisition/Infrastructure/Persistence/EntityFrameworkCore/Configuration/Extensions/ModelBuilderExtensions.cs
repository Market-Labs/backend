using marketgo_backend.Requisition.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Requisition.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
public static class ModelBuilderExtensions
{
    public static void ApplyRequisitionConfiguration(this ModelBuilder builder)
    {
        var request = builder.Entity<SupplyRequest>();
        request.ToTable("supply_requests"); request.HasKey(x => x.Id);
        request.HasIndex(x => x.MinimarketId); request.HasIndex(x => x.SupplierId);
        request.Property(x => x.Requester).HasMaxLength(160).IsRequired();
        request.Property(x => x.Supplier).HasMaxLength(160).IsRequired();
        request.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
        request.Property(x => x.Status).HasMaxLength(30).IsRequired();
        request.Property(x => x.RejectionReason).HasMaxLength(1000);
        request.Property(x => x.ItemsJson).HasColumnType("longtext");
        request.Ignore(x => x.Items); request.Ignore(x => x.DomainEvents);
    }
}
