using marketgo_backend.Analytics.Application.Services;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Analytics.Application.Internal.QueryServices;

public sealed class AnalyticsService(AppDbContext db) : IAnalyticsService
{
    public async Task<AnalyticsResource> GetAsync(Guid organizationId, CancellationToken ct)
    {
        var supplierIds = db.SupplierProfiles.Where(x => x.OrganizationId == organizationId).Select(x => x.Id);
        var stock = await db.InventoryItems.Where(x => x.OrganizationId == organizationId).SumAsync(x => (int?)x.Stock, ct) ?? 0;
        var risk = await db.InventoryItems.CountAsync(x => x.OrganizationId == organizationId && x.Stock <= x.MinimumStock, ct);
        var waste = await db.WasteRecords.Where(x => x.OrganizationId == organizationId).SumAsync(x => (int?)x.Quantity, ct) ?? 0;
        var orders = await db.PurchaseOrders.CountAsync(x => x.MinimarketId == organizationId || supplierIds.Contains(x.SupplierId), ct);
        var pending = await db.PurchaseOrders.CountAsync(x => (x.MinimarketId == organizationId || supplierIds.Contains(x.SupplierId)) && x.Status == "pending-reception", ct);
        var suppliers = await db.SupplierProfiles.CountAsync(x => x.OrganizationId == organizationId && x.Status == "active", ct);
        var conservation = await db.ConservationReadings.Where(x => x.OrganizationId == organizationId)
            .Select(x => new { x.Temperature, x.Humidity }).ToListAsync(ct);
        var healthy = conservation.Count(x => x.Temperature is >= 0 and <= 8 && x.Humidity is >= 30 and <= 80);
        var score = conservation.Count == 0 ? 100 : (int)Math.Round(100m * healthy / conservation.Count);
        var reports = new Dictionary<string, ReportSummary>
        {
            ["Inventario"] = new("Inventario", "Existencias y lotes con bajo stock.",
                [new("Unidades disponibles", stock.ToString()), new("Lotes en riesgo", risk.ToString())], []),
            ["Abastecimiento"] = new("Abastecimiento", "Ordenes de compra de la organizacion.",
                [new("Ordenes", orders.ToString()), new("Pendientes", pending.ToString())], []),
            ["Mermas"] = new("Mermas", "Unidades retiradas del inventario.",
                [new("Unidades registradas", waste.ToString())], []),
            ["Conservacion"] = new("Conservacion", "Lecturas de temperatura y humedad.",
                [new("Score", $"{score}%"), new("Lecturas", conservation.Count.ToString())], []),
            ["Proveedores"] = new("Proveedores", "Directorio de proveedores activos.",
                [new("Activos", suppliers.ToString())], [])
        };
        return new AnalyticsResource(
            [new("Inventario total", stock, 0, "items"), new("Mermas registradas", waste, 0, "units"),
             new("Score conservacion", score, 0, "%")], reports);
    }
}
