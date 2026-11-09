using System.Security.Claims;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Dashboard.Interfaces.Rest;

public sealed record ActivityResource(Guid Id, string Type, string Description, DateTimeOffset RecordedAt);

[ApiController, Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/activity-history")]
public sealed class ActivityHistoryController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirstValue("organization_id"), out var current) || current != organizationId) return Forbid();
        var waste = await db.WasteRecords.AsNoTracking().Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.RecordedAt).Take(50).Select(x => new ActivityResource(x.Id, "waste", x.ProductName, x.RecordedAt)).ToListAsync(ct);
        var donations = await db.DonationRecords.AsNoTracking().Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.RecordedAt).Take(50).Select(x => new ActivityResource(x.Id, "donation", x.ProductName, x.RecordedAt)).ToListAsync(ct);
        var orders = await db.PurchaseOrders.AsNoTracking().Where(x => x.MinimarketId == organizationId && x.CreatedAt != null)
            .OrderByDescending(x => x.CreatedAt).Take(50).Select(x => new ActivityResource(x.Id, "purchase-order", x.Status, x.CreatedAt!.Value)).ToListAsync(ct);
        return Ok(waste.Concat(donations).Concat(orders).OrderByDescending(x => x.RecordedAt).Take(100));
    }
}
