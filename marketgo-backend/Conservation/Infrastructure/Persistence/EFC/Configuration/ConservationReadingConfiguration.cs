using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace marketgo_backend.Conservation.Infrastructure.Persistence.EFC.Configuration;

public sealed class ConservationReadingConfiguration : IEntityTypeConfiguration<ConservationReading>
{
    public void Configure(EntityTypeBuilder<ConservationReading> builder)
    {
        builder.ToTable("conservation_readings"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Zone).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ProductName).IsRequired().HasMaxLength(160);
        builder.Property(x => x.Temperature).HasPrecision(5, 2);
        builder.Property(x => x.Humidity).HasPrecision(5, 2);
        builder.Ignore(x => x.Status);
        builder.HasIndex(x => new { x.OrganizationId, x.RecordedAt });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
