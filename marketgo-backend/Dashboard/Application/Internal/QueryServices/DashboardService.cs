using marketgo_backend.Dashboard.Application.Services;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Dashboard.Application.Internal.QueryServices;

public sealed class DashboardService(AppDbContext db) : IDashboardService
{
    public async Task<DashboardResource> GetAsync(Guid organizationId, CancellationToken ct)
    {
        var supplierIds = db.SupplierProfiles.Where(x => x.OrganizationId == organizationId).Select(x => x.Id);
        var stock = await db.InventoryItems.Where(x => x.OrganizationId == organizationId).SumAsync(x => (int?)x.Stock, ct) ?? 0;
        var inventoryCount = await db.InventoryItems.CountAsync(x => x.OrganizationId == organizationId, ct);
        var riskCount = await db.InventoryItems.CountAsync(x => x.OrganizationId == organizationId && x.Stock <= x.MinimumStock, ct);
        var requestCount = await db.SupplyRequests.CountAsync(x => x.MinimarketId == organizationId || supplierIds.Contains(x.SupplierId), ct);
        var orderCount = await db.PurchaseOrders.CountAsync(x => x.MinimarketId == organizationId || supplierIds.Contains(x.SupplierId), ct);
        var alerts = await db.Alerts.CountAsync(x => x.OrganizationId == organizationId && !x.Read, ct);
        var score = inventoryCount == 0 ? 100 : (int)Math.Round(100m * (inventoryCount - riskCount) / inventoryCount);
        return new DashboardResource(score,
        [
            new("Stock disponible", "inventory", stock),
            new("Requisiciones", "requisition", requestCount),
            new("Abastecimientos", "procurements", orderCount),
            new("Alertas activas", "alerts", alerts, Severity: alerts > 0 ? "warning" : "normal")
        ], []);
    }
}
