using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Communication.Interfaces.Rest;

public sealed record CreateMessageResource(string Receiver, string Subject, string Body);
public sealed record UpdateMessageResource(bool? Read, bool? Starred);
public sealed record CreateAlertResource(string Title, string Message, string Type);

[ApiController, Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/communication")]
[Route("api/v1/suppliers/{organizationId:guid}/communication")]
public sealed class CommunicationController(AppDbContext db) : ControllerBase
{
    [HttpGet("messages")]
    public async Task<IActionResult> Messages(Guid organizationId, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        return Ok(await db.Messages.AsNoTracking().Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.SentAt).Take(500).ToListAsync(ct));
    }
    [HttpPost("messages")]
    public async Task<IActionResult> Send(Guid organizationId, CreateMessageResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        if (!Guid.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId)) return Unauthorized();
        var sender = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId && x.OrganizationId == organizationId, ct);
        if (sender is null) return Unauthorized();
        Message message;
        try { message = new(organizationId, sender.FullName, resource.Receiver, resource.Subject, resource.Body); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        db.Messages.Add(message); await db.SaveChangesAsync(ct);
        return Created($"{Request.Path}/{message.Id}", message);
    }
    [HttpPatch("messages/{id:guid}")]
    public async Task<IActionResult> UpdateMessage(Guid organizationId, Guid id, UpdateMessageResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var message = await db.Messages.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId, ct);
        if (message is null) return NotFound();
        message.Update(resource.Read, resource.Starred); await db.SaveChangesAsync(ct); return Ok(message);
    }
    [HttpGet("alerts")]
    public async Task<IActionResult> Alerts(Guid organizationId, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        return Ok(await db.Alerts.AsNoTracking().Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.CreatedAt).Take(500).ToListAsync(ct));
    }
    [HttpPost("alerts")]
    public async Task<IActionResult> CreateAlert(Guid organizationId, CreateAlertResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        Alert alert;
        try { alert = new(organizationId, resource.Title, resource.Message, resource.Type); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        db.Alerts.Add(alert); await db.SaveChangesAsync(ct); return Created($"{Request.Path}/{alert.Id}", alert);
    }
    [HttpPatch("alerts/{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid organizationId, Guid id, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var alert = await db.Alerts.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId, ct);
        if (alert is null) return NotFound();
        alert.MarkRead(); await db.SaveChangesAsync(ct); return Ok(alert);
    }
    private bool Owns(Guid id) => Guid.TryParse(User.FindFirstValue("organization_id"), out var current) && current == id;
}
