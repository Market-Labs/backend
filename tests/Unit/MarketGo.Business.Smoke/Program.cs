using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.ValueObjects;
using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

var minimarket = new Organization("Tienda Verde", OrganizationKind.Minimarket);
var supplierOrganization = new Organization("Proveedor Verde", OrganizationKind.Supplier);
var supplier = new SupplierProfile(supplierOrganization.Id, "Proveedor Verde", "20123456789", "ventas@verde.pe",
    "999999999", "Lima", "Vegetales", "Lima");
var product = new Product(supplierOrganization.Id, "Tomate", "Vegetales", "Tomate organico",
    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)), 40, 5.80m, supplier.Id);
Check(product.Available && product.Price == 5.80m, "Product creation");
product.Update("Tomate fresco", 6.20m, 30, true);
Check(product.Price == 6.20m && product.Quantity == 30, "Product update");
var inventory = new InventoryItem(minimarket.Id, product.Id, product.Name, 4, 5, "L-01", product.ExpirationDate);
Check(inventory.Status == "risk", "Low stock is risky");
inventory.Update(10, 5);
Check(inventory.Status == "healthy", "Replenishment clears risk");
var request = new SupplyRequest(minimarket.Id, supplier.Id, Guid.NewGuid(), "Reponer stock",
    [new SupplyRequestItem(product.Name, 10)]);
request.Review(true, null, DateTimeOffset.UtcNow);
Check(request.Status == "accepted" && request.Items.Single().Quantity == 10, "Supplier accepts requisition");
var order = new PurchaseOrder(supplier.Id, minimarket.Id, request.Id,
    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), "Entrega en tienda",
    [new PurchaseOrderItem(product.Name, 10, product.Price)]);
request.LinkOrder(order.Id);
Check(order.Total == 62m && request.ShippingOrderId == order.Id, "Order total and requisition link");
order.ReviewReception(true, null, DateTimeOffset.UtcNow);
Check(order.Status == "received" && order.ReceivedAt is not null, "Minimarket receives order");
try { order.ReviewReception(true, null, DateTimeOffset.UtcNow); throw new Exception("Repeated reception was accepted"); }
catch (InvalidOperationException) { }
var reading = new ConservationReading(minimarket.Id, "Camara fria", "Tomate", 4m, 55m, DateTimeOffset.UtcNow);
Check(reading.Status == "healthy", "Conservation range is healthy");
var riskyReading = new ConservationReading(minimarket.Id, "Anaquel", "Tomate", 18m, 55m, DateTimeOffset.UtcNow);
Check(riskyReading.Status == "risk", "High temperature is risky");
var waste = new WasteRecord(minimarket.Id, inventory, 3, "Vencimiento");
Check(waste.Quantity == 3 && inventory.Stock == 7, "Waste subtracts stock");
try { _ = new WasteRecord(minimarket.Id, inventory, 8, "Exceso"); throw new Exception("Waste exceeded stock"); }
catch (ArgumentException) { }
var donation = new DonationRecord(minimarket.Id, inventory, 2, "Comedor local");
Check(donation.Quantity == 2 && inventory.Stock == 5, "Donation subtracts stock");
var alert = new Alert(minimarket.Id, "Stock", "Revisar lote", "warning");
alert.MarkRead();
Check(alert.Read, "Alert can be read");
var message = new Message(minimarket.Id, "A", "B", "Pedido", "Entregar");
message.Update(true, true);
Check(message.Read && message.Starred, "Message status can be updated");
minimarket.UpdateContact("Tienda Sur", "sur@example.com", "999999999", null, null, null, null);
Check(minimarket.Name == "Tienda Sur" && minimarket.Email == "sur@example.com", "Profile contact update");
using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
    .UseMySQL("server=localhost;database=marketgo;user=test;password=test").Options);
foreach (var (entity, table) in new (Type, string)[]
{
    (typeof(Product), "products"), (typeof(InventoryItem), "inventory_items"),
    (typeof(SupplierProfile), "supplier_profiles"), (typeof(SupplyRequest), "supply_requests"),
    (typeof(PurchaseOrder), "purchase_orders"),
    (typeof(WasteRecord), "waste_records"), (typeof(DonationRecord), "donation_records"),
    (typeof(ConservationReading), "conservation_readings"), (typeof(Alert), "alerts"), (typeof(Message), "messages")
}) Check(db.Model.FindEntityType(entity)?.GetTableName() == table, $"EF mapping for {table}");
Console.WriteLine("Business smoke tests passed (catalog, inventory, suppliers, requisitions, orders and EF model).");

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
