using System;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace marketgo_backend.Migrations;
[DbContext(typeof(AppDbContext))]
[Migration("20261114190000_RouteMessagesBetweenOrganizations")]
public partial class RouteMessagesBetweenOrganizations : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(name: "sender_organization_id", table: "messages",
            type: "char(36)", nullable: true);
        migrationBuilder.AddColumn<Guid>(name: "receiver_organization_id", table: "messages",
            type: "char(36)", nullable: true);
        migrationBuilder.AddColumn<Guid>(name: "idempotency_key", table: "messages",
            type: "char(36)", nullable: true);
        // The original owner is an unambiguous sender for historical rows.
        // A receiver cannot be inferred from the free-text display name.
        migrationBuilder.Sql("UPDATE messages SET sender_organization_id = organization_id WHERE sender_organization_id IS NULL");
        migrationBuilder.CreateIndex(name: "ix_messages_sender_org", table: "messages",
            column: "sender_organization_id");
        migrationBuilder.CreateIndex(name: "ix_messages_receiver_org", table: "messages",
            column: "receiver_organization_id");
        migrationBuilder.CreateIndex(name: "ux_messages_sender_key", table: "messages",
            columns: new[] { "sender_organization_id", "idempotency_key" }, unique: true);
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "ix_messages_sender_org", table: "messages");
        migrationBuilder.DropIndex(name: "ix_messages_receiver_org", table: "messages");
        migrationBuilder.DropIndex(name: "ux_messages_sender_key", table: "messages");
        migrationBuilder.DropColumn(name: "sender_organization_id", table: "messages");
        migrationBuilder.DropColumn(name: "receiver_organization_id", table: "messages");
        migrationBuilder.DropColumn(name: "idempotency_key", table: "messages");
    }
}
