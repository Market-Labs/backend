using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace marketgo_backend.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261117110000_ArchiveUnlistedOrganizations")]
public partial class ArchiveUnlistedOrganizations : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.AddColumn<bool>(
            name: "is_listed", table: "profiles", type: "tinyint(1)",
            nullable: false, defaultValue: true);

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropColumn(name: "is_listed", table: "profiles");
}
