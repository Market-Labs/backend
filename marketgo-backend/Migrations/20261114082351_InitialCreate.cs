using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace marketgo_backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    subject = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    severity = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    starred = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    sent_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_alerts", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "conservation_readings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    zone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    product_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    temperature = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    humidity = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_conservation_readings", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "donation_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    inventory_item_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    lot_code = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    recipient = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_donation_records", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inventory_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    stock = table.Column<int>(type: "int", nullable: false),
                    minimum_stock = table.Column<int>(type: "int", nullable: false),
                    lot_code = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    expiration_date = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_inventory_items", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    sender = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    receiver = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    subject = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false),
                    read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    starred = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    sent_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_messages", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    supplier_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    category = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    expiration_date = table.Column<DateOnly>(type: "date", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    available = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_products", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "profiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    owner_email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    business_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    phone = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    address = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    district = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    specialty = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    coverage_area = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_profiles", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "purchase_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    supplier_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    minimarket_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    supply_request_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    supplier = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    minimarket = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    shipping_date = table.Column<DateOnly>(type: "date", nullable: false),
                    observations = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    total = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    reviewed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    reviewed_by_user_id = table.Column<int>(type: "int", nullable: true),
                    rejection_reason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    items_json = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_purchase_orders", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "supplier_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    business_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    ruc = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    phone = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    address = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    specialty = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    coverage_area = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_supplier_profiles", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "supply_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    minimarket_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    supplier_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    requester_id = table.Column<int>(type: "int", nullable: false),
                    requester = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    supplier = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    reason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    rejection_reason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    shipping_order_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    items_json = table.Column<string>(type: "longtext", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_supply_requests", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    full_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    normalized_email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    organization_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    account_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    role = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_signed_in_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "waste_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false),
                    organization_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    inventory_item_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    product_name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    lot_code = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_waste_records", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_alerts_organization_id",
                table: "alerts",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "i_x_conservation_readings_organization_id_recorded_at",
                table: "conservation_readings",
                columns: new[] { "organization_id", "recorded_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_donation_records_organization_id",
                table: "donation_records",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "i_x_inventory_items_organization_id_lot_code",
                table: "inventory_items",
                columns: new[] { "organization_id", "lot_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_messages_organization_id",
                table: "messages",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "i_x_products_organization_id",
                table: "products",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "i_x_profiles_organization_id",
                table: "profiles",
                column: "organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_purchase_orders_minimarket_id",
                table: "purchase_orders",
                column: "minimarket_id");

            migrationBuilder.CreateIndex(
                name: "i_x_purchase_orders_supplier_id",
                table: "purchase_orders",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "i_x_purchase_orders_supply_request_id",
                table: "purchase_orders",
                column: "supply_request_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_supplier_profiles_organization_id_ruc",
                table: "supplier_profiles",
                columns: new[] { "organization_id", "ruc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_supply_requests_minimarket_id",
                table: "supply_requests",
                column: "minimarket_id");

            migrationBuilder.CreateIndex(
                name: "i_x_supply_requests_supplier_id",
                table: "supply_requests",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "i_x_users_normalized_email",
                table: "users",
                column: "normalized_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_organization_id",
                table: "users",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "i_x_waste_records_organization_id",
                table: "waste_records",
                column: "organization_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.DropTable(
                name: "conservation_readings");

            migrationBuilder.DropTable(
                name: "donation_records");

            migrationBuilder.DropTable(
                name: "inventory_items");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "profiles");

            migrationBuilder.DropTable(
                name: "purchase_orders");

            migrationBuilder.DropTable(
                name: "supplier_profiles");

            migrationBuilder.DropTable(
                name: "supply_requests");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "waste_records");
        }
    }
}
