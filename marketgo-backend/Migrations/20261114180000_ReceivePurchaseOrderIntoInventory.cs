using System;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace marketgo_backend.Migrations;
[DbContext(typeof(AppDbContext))]
[Migration("20261114180000_ReceivePurchaseOrderIntoInventory")]
public partial class ReceivePurchaseOrderIntoInventory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "inventory_receipt_movements",
            columns: table => new
            {
                id = table.Column<Guid>(type: "char(36)", nullable: false),
                organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                purchase_order_id = table.Column<Guid>(type: "char(36)", nullable: false),
                line_index = table.Column<int>(type: "int", nullable: false),
                inventory_item_id = table.Column<Guid>(type: "char(36)", nullable: false),
                product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                lot_code = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                expiration_date = table.Column<DateOnly>(type: "date", nullable: false),
                quantity = table.Column<int>(type: "int", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
            },
            constraints: table => table.PrimaryKey("p_k_inventory_receipt_movements", x => x.id));
        migrationBuilder.CreateIndex(
            name: "ux_receipt_order_line_lot",
            table: "inventory_receipt_movements",
            columns: new[] { "purchase_order_id", "line_index", "lot_code" },
            unique: true);
        migrationBuilder.CreateIndex(
            name: "ix_receipt_org_inventory",
            table: "inventory_receipt_movements",
            columns: new[] { "organization_id", "inventory_item_id" });
    }
    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "inventory_receipt_movements");
}
