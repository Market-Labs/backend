using System.Security.Claims;
using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Inventory.Interfaces.Rest;

public sealed record CreateWasteResource(Guid InventoryItemId, int Quantity, string Reason);

[ApiController, Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/waste")]
[Route("api/v1/suppliers/{organizationId:guid}/waste")]
public sealed class WasteController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        return Ok(await db.WasteRecords.AsNoTracking().Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.RecordedAt).Take(500).ToListAsync(ct));
    }
    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, CreateWasteResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var item = await db.InventoryItems.FirstOrDefaultAsync(x => x.Id == resource.InventoryItemId && x.OrganizationId == organizationId, ct);
        if (item is null) return NotFound();
        WasteRecord record;
        try { record = new(organizationId, item, resource.Quantity, resource.Reason); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        db.WasteRecords.Add(record); await db.SaveChangesAsync(ct);
        return Created($"{Request.Path}/{record.Id}", record);
    }
    private bool Owns(Guid id) => Guid.TryParse(User.FindFirstValue("organization_id"), out var current) && current == id;
}
