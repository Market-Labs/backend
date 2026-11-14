using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyIamConfiguration(this ModelBuilder builder)
    {
        builder.Entity<User>().ToTable("users");
        builder.Entity<User>().HasKey(user => user.Id);
        builder.Entity<User>().Property(user => user.Id)
            .HasConversion(new StronglyTypedIdConverter<UserId, int>(id => id.Value, value => new UserId(value)))
            .ValueGeneratedOnAdd();
        builder.Entity<User>().Property(user => user.FullName).IsRequired().HasMaxLength(160);
        builder.Entity<User>().Property(user => user.Email).IsRequired().HasMaxLength(254);
        builder.Entity<User>().Property(user => user.NormalizedEmail).IsRequired().HasMaxLength(254);
        builder.Entity<User>().HasIndex(user => user.NormalizedEmail).IsUnique();
        builder.Entity<User>().Property(user => user.PasswordHash).IsRequired().HasMaxLength(200);
        builder.Entity<User>().Property(user => user.OrganizationName).IsRequired().HasMaxLength(160);
        builder.Entity<User>().Property(user => user.AccountType).IsRequired().HasMaxLength(20);
        builder.Entity<User>().Property(user => user.Role).IsRequired().HasMaxLength(40);
        builder.Entity<User>().HasIndex(user => user.OrganizationId);
        builder.Entity<User>().Ignore(user => user.DomainEvents);
    }
}
