using marketgo_backend.Analytics.Application.QueryServices;
using marketgo_backend.Analytics.Domain.Model.Queries;
using marketgo_backend.Analytics.Domain.Model.ValueObjects;
using marketgo_backend.Conservation.Interfaces.Acl;
using marketgo_backend.Inventory.Interfaces.Acl;
using marketgo_backend.Procurements.Interfaces.Acl;
using marketgo_backend.Suppliers.Interfaces.Acl;
namespace marketgo_backend.Analytics.Application.Internal.QueryServices;
public sealed class AnalyticsQueryService(IInventoryContextFacade inventory,
    IProcurementsContextFacade procurements, IConservationContextFacade conservation,
    ISuppliersContextFacade suppliers) : IAnalyticsQueryService
{
    public async Task<AnalyticsSnapshot> Handle(GetAnalyticsQuery query, CancellationToken ct)
    {
        var stock = await inventory.GetMetricsAsync(query.OrganizationId, ct);
        var orders = await procurements.GetMetricsAsync(query.OrganizationId, query.AccountType, ct);
        var readings = await conservation.GetMetricsAsync(query.OrganizationId, ct);
        var supplierCount = await suppliers.CountActiveAsync(query.OrganizationId, ct);
        var conservationScore = readings.TotalReadings == 0 ? 100 :
            (int)Math.Round(100m * readings.HealthyReadings / readings.TotalReadings);
        OperationalIndicator[] indicators =
        [
            new("Inventario total", stock.Stock, 0, "items"),
            new("Mermas registradas", stock.WasteQuantity, 0, "units"),
            new("Score conservación", conservationScore, 0, "%")
        ];
        Dictionary<string, ReportSummary> reports = new()
        {
            ["Inventario"] = new("Inventario", "Existencias y lotes con bajo stock.",
                [new("Unidades disponibles", stock.Stock.ToString()), new("Lotes en riesgo", stock.RiskCount.ToString())]),
            ["Abastecimiento"] = new("Abastecimiento", "Órdenes de la organización.",
                [new("Órdenes", orders.TotalOrders.ToString()), new("Pendientes", orders.PendingReception.ToString())]),
            ["Mermas"] = new("Mermas", "Unidades retiradas del inventario.",
                [new("Unidades registradas", stock.WasteQuantity.ToString())]),
            ["Conservacion"] = new("Conservación", "Lecturas recientes de temperatura y humedad.",
                [new("Score", $"{conservationScore}%"), new("Lecturas", readings.TotalReadings.ToString())]),
            ["Proveedores"] = new("Proveedores", "Directorio de proveedores activos.",
                [new("Activos", supplierCount.ToString())])
        };
        return new AnalyticsSnapshot(indicators, reports);
    }
}
