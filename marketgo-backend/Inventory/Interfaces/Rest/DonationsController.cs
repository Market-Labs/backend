using System.Security.Claims;
using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Inventory.Interfaces.Rest;

public sealed record CreateDonationResource(Guid InventoryItemId, int Quantity, string Recipient);

[ApiController, Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/donations")]
public sealed class DonationsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        return Ok(await db.DonationRecords.AsNoTracking().Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.RecordedAt).Take(500).ToListAsync(ct));
    }
    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, CreateDonationResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var item = await db.InventoryItems.FirstOrDefaultAsync(x => x.Id == resource.InventoryItemId && x.OrganizationId == organizationId, ct);
        if (item is null) return NotFound();
        DonationRecord donation;
        try { donation = new(organizationId, item, resource.Quantity, resource.Recipient); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        db.DonationRecords.Add(donation); await db.SaveChangesAsync(ct);
        return Created($"{Request.Path}/{donation.Id}", donation);
    }
    private bool Owns(Guid id) => Guid.TryParse(User.FindFirstValue("organization_id"), out var current) && current == id;
}
