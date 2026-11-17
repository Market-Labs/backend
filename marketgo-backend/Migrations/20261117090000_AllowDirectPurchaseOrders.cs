using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace marketgo_backend.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261117090000_AllowDirectPurchaseOrders")]
public partial class AllowDirectPurchaseOrders : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "supply_request_id",
            table: "purchase_orders",
            type: "char(36)",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "char(36)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        throw new NotSupportedException("Direct purchase orders must be preserved; this migration cannot be reversed automatically.");
    }
}
