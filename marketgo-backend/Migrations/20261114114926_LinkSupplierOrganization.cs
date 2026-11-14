using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace marketgo_backend.Migrations
{
    /// <inheritdoc />
    public partial class LinkSupplierOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "supplier_organization_id",
                table: "supplier_profiles",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_supplier_profiles_organization_id_supplier_organization_id",
                table: "supplier_profiles",
                columns: new[] { "organization_id", "supplier_organization_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_supplier_profiles_organization_id_supplier_organization_id",
                table: "supplier_profiles");

            migrationBuilder.DropColumn(
                name: "supplier_organization_id",
                table: "supplier_profiles");
        }
    }
}
