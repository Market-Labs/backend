using marketgo_backend.Organizations.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace marketgo_backend.Organizations.Infrastructure.Persistence.EFC.Configuration;

public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");
        builder.HasKey(organization => organization.Id);
        builder.Property(organization => organization.Id).ValueGeneratedNever();
        builder.Property(organization => organization.Name).IsRequired().HasMaxLength(160);
        builder.Property(organization => organization.Kind).HasConversion<string>().IsRequired().HasMaxLength(32);
        builder.Property(organization => organization.CreatedAt);
        builder.Property(organization => organization.UpdatedAt);
    }
}
