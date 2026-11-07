using System.Security.Claims;
using marketgo_backend.Organizations.Domain.Model.ValueObjects;
using marketgo_backend.ProcurementOrders.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.ProcurementOrders.Interfaces.Rest;

public sealed record CreateSupplyRequestResource(Guid SupplierId, string Reason, IReadOnlyList<SupplyRequestItem> Items);
public sealed record ReviewSupplyRequestResource(string Status, string? RejectionReason);

[ApiController]
[Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/requisitions")]
[Route("api/v1/suppliers/{organizationId:guid}/requisitions")]
public sealed class RequisitionsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var supplierIds = db.SupplierProfiles.Where(x => x.OrganizationId == organizationId).Select(x => x.Id);
        return Ok(await db.SupplyRequests.AsNoTracking()
            .Where(x => x.MinimarketId == organizationId || supplierIds.Contains(x.SupplierId))
            .OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var supplierIds = db.SupplierProfiles.Where(x => x.OrganizationId == organizationId).Select(x => x.Id);
        var request = await db.SupplyRequests.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && (x.MinimarketId == organizationId || supplierIds.Contains(x.SupplierId)), cancellationToken);
        return request is null ? NotFound() : Ok(request);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, CreateSupplyRequestResource resource, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var organization = await db.Organizations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == organizationId, cancellationToken);
        if (organization?.Kind != OrganizationKind.Minimarket) return BadRequest(new { error = "Only minimarkets can create requisitions." });
        if (!Guid.TryParse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub), out var requesterId)) return Unauthorized();
        if (!await db.SupplierProfiles.AnyAsync(x => x.Id == resource.SupplierId && x.Status == "active", cancellationToken))
            return BadRequest(new { error = "Supplier does not exist or is inactive." });
        SupplyRequest request;
        try { request = new SupplyRequest(organizationId, resource.SupplierId, requesterId, resource.Reason, resource.Items ?? []); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        db.SupplyRequests.Add(request);
        await db.SaveChangesAsync(cancellationToken);
        return Created($"{Request.Path}/{request.Id}", request);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Review(Guid organizationId, Guid id, ReviewSupplyRequestResource resource, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var request = await db.SupplyRequests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (request is null) return NotFound();
        var supplierOwns = await db.SupplierProfiles.AnyAsync(x => x.Id == request.SupplierId && x.OrganizationId == organizationId, cancellationToken);
        if (!supplierOwns) return Forbid();
        if (resource.Status is not ("accepted" or "rejected")) return BadRequest(new { error = "Status must be accepted or rejected." });
        try { request.Review(resource.Status == "accepted", resource.RejectionReason, DateTimeOffset.UtcNow); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        catch (InvalidOperationException error) { return Conflict(new { error = error.Message }); }
        await db.SaveChangesAsync(cancellationToken);
        return Ok(request);
    }

    private bool Owns(Guid organizationId) => Guid.TryParse(User.FindFirstValue("organization_id"), out var current) && current == organizationId;
}
