using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Organizations.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace marketgo_backend.Iam.Infrastructure.Persistence.EFC.Configuration;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();
        builder.Property(user => user.FullName).IsRequired().HasMaxLength(160);
        builder.Property(user => user.Email).IsRequired().HasMaxLength(254);
        builder.Property(user => user.NormalizedEmail).IsRequired().HasMaxLength(254);
        builder.HasIndex(user => user.NormalizedEmail).IsUnique();
        builder.Property(user => user.PasswordHash).IsRequired().HasMaxLength(256);
        builder.Property(user => user.Role).IsRequired().HasMaxLength(32);
        builder.Property(user => user.IsActive).IsRequired();
        builder.HasOne<Organization>().WithMany().HasForeignKey(user => user.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
