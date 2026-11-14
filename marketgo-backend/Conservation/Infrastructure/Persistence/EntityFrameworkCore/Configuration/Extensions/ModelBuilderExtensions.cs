using marketgo_backend.Conservation.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Conservation.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
public static class ModelBuilderExtensions
{
    public static void ApplyConservationConfiguration(this ModelBuilder builder)
    {
        var reading = builder.Entity<ConservationReading>();
        reading.ToTable("conservation_readings"); reading.HasKey(x => x.Id);
        reading.HasIndex(x => new { x.OrganizationId, x.RecordedAt });
        reading.Property(x => x.Zone).HasMaxLength(100).IsRequired();
        reading.Property(x => x.ProductName).HasMaxLength(160).IsRequired();
        reading.Property(x => x.Temperature).HasPrecision(6, 2);
        reading.Property(x => x.Humidity).HasPrecision(5, 2);
        reading.Ignore(x => x.Status); reading.Ignore(x => x.DomainEvents);
    }
}
