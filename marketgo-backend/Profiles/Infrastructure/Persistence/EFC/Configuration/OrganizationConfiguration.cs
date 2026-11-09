using marketgo_backend.Profiles.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace marketgo_backend.Profiles.Infrastructure.Persistence.EFC.Configuration;

public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");
        builder.HasKey(organization => organization.Id);
        builder.Property(organization => organization.Id).ValueGeneratedNever();
        builder.Property(organization => organization.Name).IsRequired().HasMaxLength(160);
        builder.Property(organization => organization.Email).HasMaxLength(254);
        builder.Property(organization => organization.Phone).HasMaxLength(40);
        builder.Property(organization => organization.Address).HasMaxLength(250);
        builder.Property(organization => organization.District).HasMaxLength(100);
        builder.Property(organization => organization.Specialty).HasMaxLength(100);
        builder.Property(organization => organization.CoverageArea).HasMaxLength(150);
        builder.Property(organization => organization.Kind).HasConversion<string>().IsRequired().HasMaxLength(32);
        builder.Property(organization => organization.CreatedAt);
        builder.Property(organization => organization.UpdatedAt);
    }
}
