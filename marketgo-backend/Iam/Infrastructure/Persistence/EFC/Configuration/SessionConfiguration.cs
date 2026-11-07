using marketgo_backend.Iam.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace marketgo_backend.Iam.Infrastructure.Persistence.EFC.Configuration;

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");
        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).ValueGeneratedNever();
        builder.Property(session => session.TokenHash).IsRequired().HasMaxLength(64);
        builder.HasIndex(session => session.TokenHash).IsUnique();
        builder.HasIndex(session => session.UserId);
        builder.HasOne<User>().WithMany().HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
