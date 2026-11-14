using marketgo_backend.Communication.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Communication.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
public static class ModelBuilderExtensions
{
    public static void ApplyCommunicationConfiguration(this ModelBuilder builder)
    {
        var alert = builder.Entity<Alert>();
        alert.ToTable("alerts"); alert.HasKey(x => x.Id); alert.HasIndex(x => x.OrganizationId);
        alert.Property(x => x.Subject).HasMaxLength(200).IsRequired();
        alert.Property(x => x.Body).HasMaxLength(2000).IsRequired();
        alert.Property(x => x.Severity).HasMaxLength(20).IsRequired();
        alert.Ignore(x => x.Sender); alert.Ignore(x => x.DomainEvents);
        var message = builder.Entity<Message>();
        message.ToTable("messages"); message.HasKey(x => x.Id); message.HasIndex(x => x.OrganizationId);
        message.Property(x => x.Sender).HasMaxLength(160).IsRequired();
        message.Property(x => x.Receiver).HasMaxLength(160).IsRequired();
        message.Property(x => x.Subject).HasMaxLength(200).IsRequired();
        message.Property(x => x.Body).HasMaxLength(4000).IsRequired();
        message.Ignore(x => x.DomainEvents);
    }
}
