using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace marketgo_backend.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261117130000_BackfillInitialInventory")]
public partial class BackfillInitialInventory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        INSERT INTO inventory_items
            (id, organization_id, product_id, product_name, stock, minimum_stock,
             lot_code, expiration_date, created_at, updated_at)
        SELECT UUID(), p.organization_id, p.id, p.name, p.quantity, 0,
               CONCAT('INIT-', REPLACE(p.id, '-', '')), p.expiration_date, UTC_TIMESTAMP(), NULL
        FROM products p
        WHERE p.available = 1
          AND NOT EXISTS (
              SELECT 1 FROM inventory_items i
              WHERE i.organization_id = p.organization_id AND i.product_id = p.id
          );
        """);

    protected override void Down(MigrationBuilder migrationBuilder) =>
        throw new NotSupportedException("Inventory movements may exist; initial lots cannot be removed safely.");
}
