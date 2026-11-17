using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace marketgo_backend.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261117120000_AllowExternalClientOrders")]
public partial class AllowExternalClientOrders : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "minimarket_id", table: "purchase_orders", type: "char(36)",
            nullable: true, oldClrType: typeof(Guid), oldType: "char(36)");
        migrationBuilder.AddColumn<Guid>(
            name: "client_contact_id", table: "purchase_orders", type: "char(36)", nullable: true);
        migrationBuilder.CreateIndex(
            name: "i_x_purchase_orders_client_contact_id", table: "purchase_orders",
            column: "client_contact_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        throw new NotSupportedException("External client orders must be preserved.");
}
