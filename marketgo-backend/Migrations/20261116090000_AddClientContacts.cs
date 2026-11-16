using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace marketgo_backend.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261116090000_AddClientContacts")]
public partial class AddClientContacts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "client_contacts",
            columns: table => new
            {
                id = table.Column<Guid>(type: "char(36)", nullable: false),
                supplier_organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                business_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                phone = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                address = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                district = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
            },
            constraints: table => table.PrimaryKey("p_k_client_contacts", x => x.id));
        migrationBuilder.CreateIndex(
            name: "i_x_client_contacts_supplier_organization_id",
            table: "client_contacts",
            column: "supplier_organization_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "client_contacts");
}