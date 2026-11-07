using marketgo_backend.Organizations.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace marketgo_backend.Organizations.Infrastructure.Persistence.EFC.Configuration;

public sealed class SupplierProfileConfiguration : IEntityTypeConfiguration<SupplierProfile>
{
    public void Configure(EntityTypeBuilder<SupplierProfile> builder)
    {
        builder.ToTable("supplier_profiles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.BusinessName).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Ruc).HasMaxLength(11).IsRequired();
        builder.HasIndex(x => x.Ruc).IsUnique();
        builder.Property(x => x.Email).HasMaxLength(254).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Specialty).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CoverageArea).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(16).IsRequired();
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
