using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Domain.Model.ValueObjects;
using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

var minimarket = Guid.NewGuid();
var supplierOrg = Guid.NewGuid();
var supplier = new SupplierProfile(minimarket, "Proveedor Andino", "20600011122",
    "ventas@andino.pe", "999111222", "Lima", "Lácteos", "Lima");
Check(supplier.Status == "active" && supplier.DomainEvents.Count == 1, "Supplier event and status");
try { new SupplierProfile(minimarket, "Incorrecto", "123", "bad", "x", "x", "x", "x"); throw new Exception("Invalid RUC accepted"); }
catch (ArgumentException) { }
var product = new Product(supplierOrg, "Yogurt", "Lácteos", "Yogurt natural",
    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)), 20, 8.5m, null);
Check(product.DomainEvents.Count == 1 && product.Price == 8.5m, "Product creation");
product.Withdraw();
Check(!product.Available, "Product withdrawal");
var item = new InventoryItem(minimarket, product.Id, product.Name, 20, 5,
    "LOT-001", product.ExpirationDate);
var waste = new WasteRecord(minimarket, item, 3, "Dañado");
Check(item.Stock == 17 && waste.Quantity == 3, "Waste decrements stock");
var donation = new DonationRecord(minimarket, item, 2, "Comedor");
Check(item.Stock == 15 && donation.Quantity == 2, "Donation decrements stock");
try { item.RemoveStock(16); throw new Exception("Insufficient stock accepted"); }
catch (ArgumentException) { }
var request = new SupplyRequest(minimarket, supplierOrg, 1, "Ana", "Andino",
    "Reponer lácteos", [new SupplyRequestItem("Yogurt", 5)]);
Check(request.Status == "pending", "Request starts pending");
request.Review("accepted", null, DateTimeOffset.UtcNow);
Check(request.Status == "accepted" && request.DomainEvents.Count == 1, "Supplier accepts request");
var order = new PurchaseOrder(supplierOrg, minimarket, request.Id, "Andino", "Tienda",
    DateOnly.FromDateTime(DateTime.Today.AddDays(2)), null,
    [new PurchaseOrderItem("Yogurt", 5, 8.5m)]);
request.LinkOrder(order.Id);
Check(order.Total == 42.5m && request.ShippingOrderId == order.Id, "Order links request and computes total");
order.ReviewReception("received", null, 1, DateTimeOffset.UtcNow);
Check(order.Status == "received" && order.ReceivedAt.HasValue, "Minimarket receives order");
try { order.ReviewReception("rejected", "Late", 1, DateTimeOffset.UtcNow); throw new Exception("Double review accepted"); }
catch (InvalidOperationException) { }
var reading = new ConservationReading(minimarket, "Frío", "Yogurt", 15m, 50m, DateTimeOffset.UtcNow);
Check(reading.Status == "risk" && reading.DomainEvents.Count == 1, "Risk reading emits event");
var alert = new Alert(minimarket, "Temperatura", "Fuera de rango", "warning");
alert.Update(true, null);
Check(alert.Read, "Alert read state");
var message = new Message(minimarket, "Ana", "Andino", "Pedido", "Gracias");
message.Update(null, true);
Check(message.Starred, "Message starred state");
using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
    .UseMySQL("server=localhost;database=marketgo;user=test;password=test").Options);
foreach (var (entity, table) in new (Type, string)[] {
    (typeof(SupplierProfile), "supplier_profiles"), (typeof(Product), "products"),
    (typeof(InventoryItem), "inventory_items"), (typeof(WasteRecord), "waste_records"),
    (typeof(DonationRecord), "donation_records"), (typeof(SupplyRequest), "supply_requests"),
    (typeof(PurchaseOrder), "purchase_orders"), (typeof(ConservationReading), "conservation_readings"),
    (typeof(Alert), "alerts"), (typeof(Message), "messages") })
    Check(db.Model.FindEntityType(entity)?.GetTableName() == table, $"EF mapping for {table}");
Console.WriteLine("Business smoke passed: domain rules, transitions, stock movements, events and EF mappings.");
static void Check(bool value, string message) { if (!value) throw new Exception(message); }
