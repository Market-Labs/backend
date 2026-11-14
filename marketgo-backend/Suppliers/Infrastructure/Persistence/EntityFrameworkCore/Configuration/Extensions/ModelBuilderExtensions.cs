using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Suppliers.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
public static class ModelBuilderExtensions
{
    public static void ApplySuppliersConfiguration(this ModelBuilder builder)
    {
        var supplier = builder.Entity<SupplierProfile>();
        builder.Entity<SupplierProfile>().Ignore(x => x.DomainEvents);
        supplier.ToTable("supplier_profiles");
        supplier.HasKey(x => x.Id);
        supplier.HasIndex(x => new { x.OrganizationId, x.Ruc }).IsUnique();
        supplier.Property(x => x.BusinessName).HasMaxLength(160).IsRequired();
        supplier.Property(x => x.Ruc).HasMaxLength(11).IsRequired();
        supplier.Property(x => x.Email).HasMaxLength(254).IsRequired();
        supplier.Property(x => x.Phone).HasMaxLength(40).IsRequired();
        supplier.Property(x => x.Address).HasMaxLength(250).IsRequired();
        supplier.Property(x => x.Specialty).HasMaxLength(100).IsRequired();
        supplier.Property(x => x.CoverageArea).HasMaxLength(150).IsRequired();
        supplier.Property(x => x.Status).HasMaxLength(20).IsRequired();
    }
}
