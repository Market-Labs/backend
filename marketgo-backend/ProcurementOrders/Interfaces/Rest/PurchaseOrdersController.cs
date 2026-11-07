using System.Security.Claims;
using marketgo_backend.ProcurementOrders.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.ProcurementOrders.Interfaces.Rest;

public sealed record CreatePurchaseOrderResource(Guid SupplierId, Guid MinimarketId, Guid? SupplyRequestId,
    DateOnly ShippingDate, string Observations, IReadOnlyList<PurchaseOrderItem> Items);
public sealed record ReviewPurchaseOrderResource(string Status, string? RejectionReason);

[ApiController]
[Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/purchase-orders")]
[Route("api/v1/suppliers/{organizationId:guid}/purchase-orders")]
public sealed class PurchaseOrdersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var supplierIds = db.SupplierProfiles.Where(x => x.OrganizationId == organizationId).Select(x => x.Id);
        return Ok(await db.PurchaseOrders.AsNoTracking()
            .Where(x => x.MinimarketId == organizationId || supplierIds.Contains(x.SupplierId))
            .OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var supplierIds = db.SupplierProfiles.Where(x => x.OrganizationId == organizationId).Select(x => x.Id);
        var order = await db.PurchaseOrders.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && (x.MinimarketId == organizationId || supplierIds.Contains(x.SupplierId)), cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, CreatePurchaseOrderResource resource, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var supplier = await db.SupplierProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == resource.SupplierId && x.OrganizationId == organizationId && x.Status == "active", cancellationToken);
        if (supplier is null) return Forbid();
        if (!await db.Organizations.AnyAsync(x => x.Id == resource.MinimarketId &&
            x.Kind == marketgo_backend.Organizations.Domain.Model.ValueObjects.OrganizationKind.Minimarket, cancellationToken))
            return BadRequest(new { error = "Minimarket does not exist." });
        SupplyRequest? request = null;
        if (resource.SupplyRequestId is Guid requestId)
        {
            request = await db.SupplyRequests.FirstOrDefaultAsync(x => x.Id == requestId && x.SupplierId == supplier.Id && x.MinimarketId == resource.MinimarketId, cancellationToken);
            if (request is null || request.Status != "accepted" || request.ShippingOrderId is not null)
                return Conflict(new { error = "Request is not accepted or already has an order." });
        }
        PurchaseOrder order;
        try { order = new PurchaseOrder(supplier.Id, resource.MinimarketId, request?.Id, resource.ShippingDate,
            resource.Observations ?? string.Empty, resource.Items ?? []); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        catch (OverflowException) { return BadRequest(new { error = "Order total is too large." }); }
        if (request is not null) request.LinkOrder(order.Id);
        db.PurchaseOrders.Add(order);
        await db.SaveChangesAsync(cancellationToken);
        return Created($"{Request.Path}/{order.Id}", order);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Review(Guid organizationId, Guid id, ReviewPurchaseOrderResource resource, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var order = await db.PurchaseOrders.FirstOrDefaultAsync(x => x.Id == id && x.MinimarketId == organizationId, cancellationToken);
        if (order is null) return NotFound();
        if (resource.Status is not ("received" or "rejected")) return BadRequest(new { error = "Status must be received or rejected." });
        try { order.ReviewReception(resource.Status == "received", resource.RejectionReason, DateTimeOffset.UtcNow); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        catch (InvalidOperationException error) { return Conflict(new { error = error.Message }); }
        await db.SaveChangesAsync(cancellationToken);
        return Ok(order);
    }

    private bool Owns(Guid organizationId) => Guid.TryParse(User.FindFirstValue("organization_id"), out var current) && current == organizationId;
}
