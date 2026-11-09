using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Conservation.Interfaces.Rest;

public sealed record CreateConservationReadingResource([Required] string Zone, [Required] string ProductName,
    decimal Temperature, decimal Humidity, DateTimeOffset? RecordedAt);

[ApiController, Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/conservation/monitoring")]
[Route("api/v1/suppliers/{organizationId:guid}/conservation/monitoring")]
public sealed class ConservationController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var records = await db.ConservationReadings.AsNoTracking().Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.RecordedAt).Take(500).ToListAsync(cancellationToken);
        return Ok(records);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, CreateConservationReadingResource resource, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        ConservationReading reading;
        try { reading = new(organizationId, resource.Zone, resource.ProductName, resource.Temperature,
            resource.Humidity, resource.RecordedAt ?? DateTimeOffset.UtcNow); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        db.ConservationReadings.Add(reading);
        await db.SaveChangesAsync(cancellationToken);
        return Created($"{Request.Path}/{reading.Id}", reading);
    }

    private bool Owns(Guid id) => Guid.TryParse(User.FindFirstValue("organization_id"), out var current) && current == id;
}
