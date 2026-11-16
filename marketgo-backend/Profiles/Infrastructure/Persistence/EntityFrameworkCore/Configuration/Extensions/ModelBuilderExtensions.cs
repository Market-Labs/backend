using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyProfilesConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Profile>().ToTable("profiles");
        builder.Entity<Profile>().HasKey(profile => profile.Id);
        builder.Entity<Profile>().Property(profile => profile.Id)
            .HasConversion(new StronglyTypedIdConverter<ProfileId, int>(id => id.Value, value => new ProfileId(value)))
            .ValueGeneratedOnAdd();
        builder.Entity<Profile>().HasIndex(profile => profile.OrganizationId).IsUnique();
        builder.Entity<Profile>().Property(profile => profile.OwnerEmail).IsRequired().HasMaxLength(254);
        builder.Entity<Profile>().Property(profile => profile.Type).IsRequired().HasMaxLength(20);
        builder.Entity<Profile>().Property(profile => profile.BusinessName).IsRequired().HasMaxLength(160);
        builder.Entity<Profile>().Property(profile => profile.Email).IsRequired().HasMaxLength(254);
        builder.Entity<Profile>().Property(profile => profile.Phone).HasMaxLength(40);
        builder.Entity<Profile>().Property(profile => profile.Address).HasMaxLength(250);
        builder.Entity<Profile>().Property(profile => profile.District).HasMaxLength(120);
        builder.Entity<Profile>().Property(profile => profile.Specialty).HasMaxLength(160);
        builder.Entity<Profile>().Property(profile => profile.CoverageArea).HasMaxLength(160);
        builder.Entity<Profile>().Ignore(profile => profile.DomainEvents);
        var client = builder.Entity<ClientContact>();
        client.ToTable("client_contacts");
        client.HasKey(contact => contact.Id);
        client.HasIndex(contact => contact.SupplierOrganizationId);
        client.Property(contact => contact.BusinessName).IsRequired().HasMaxLength(160);
        client.Property(contact => contact.Email).IsRequired().HasMaxLength(254);
        client.Property(contact => contact.Phone).IsRequired().HasMaxLength(40);
        client.Property(contact => contact.Address).IsRequired().HasMaxLength(250);
        client.Property(contact => contact.District).IsRequired().HasMaxLength(120);
    }
}
