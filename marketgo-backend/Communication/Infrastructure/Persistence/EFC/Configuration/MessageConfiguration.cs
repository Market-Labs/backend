using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace marketgo_backend.Communication.Infrastructure.Persistence.EFC.Configuration;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages"); builder.HasKey(x => x.Id); builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Sender).IsRequired().HasMaxLength(160);
        builder.Property(x => x.Receiver).IsRequired().HasMaxLength(160);
        builder.Property(x => x.Subject).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Body).IsRequired().HasMaxLength(4000);
        builder.HasIndex(x => new { x.OrganizationId, x.SentAt });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
