using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using marketgo_backend.CatalogInventory.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.CatalogInventory.Interfaces.Rest;

public sealed record CreateInventoryItemResource(
    Guid ProductId,
    [Range(0, int.MaxValue)] int Stock,
    [Range(0, int.MaxValue)] int MinimumStock,
    [Required, StringLength(80, MinimumLength = 1)] string LotCode,
    DateOnly ExpirationDate);

public sealed record UpdateInventoryItemResource(int? Stock, int? MinimumStock);

[ApiController]
[Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/inventory")]
[Route("api/v1/suppliers/{organizationId:guid}/inventory")]
public sealed class InventoryController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        return Ok(await db.InventoryItems.AsNoTracking().Where(x => x.OrganizationId == organizationId)
            .OrderBy(x => x.ProductName).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var item = await db.InventoryItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(Guid organizationId, [FromQuery] string? query, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var items = db.InventoryItems.AsNoTracking().Where(x => x.OrganizationId == organizationId);
        if (!string.IsNullOrWhiteSpace(query)) items = items.Where(x => x.ProductName.Contains(query) || x.LotCode.Contains(query));
        return Ok(await items.OrderBy(x => x.ProductName).ToListAsync(cancellationToken));
    }

    [HttpGet("lots")]
    public Task<IActionResult> Lots(Guid organizationId, CancellationToken cancellationToken) => List(organizationId, cancellationToken);

    [HttpGet("expirations")]
    public async Task<IActionResult> Expirations(Guid organizationId, [FromQuery] int days = 7, CancellationToken cancellationToken = default)
    {
        if (!Owns(organizationId)) return Forbid();
        if (days is < 0 or > 365) return BadRequest(new { error = "Days must be between 0 and 365." });
        var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(days));
        return Ok(await db.InventoryItems.AsNoTracking().Where(x => x.OrganizationId == organizationId && x.ExpirationDate <= cutoff)
            .OrderBy(x => x.ExpirationDate).ToListAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, CreateInventoryItemResource resource, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == resource.ProductId && x.Available, cancellationToken);
        if (product is null) return BadRequest(new { error = "Product does not exist or is unavailable." });
        InventoryItem item;
        try { item = new InventoryItem(organizationId, product.Id, product.Name, resource.Stock,
            resource.MinimumStock, resource.LotCode, resource.ExpirationDate); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        db.InventoryItems.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return Created($"{Request.Path}/{item.Id}", item);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid organizationId, Guid id, UpdateInventoryItemResource resource, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var item = await db.InventoryItems.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId, cancellationToken);
        if (item is null) return NotFound();
        try { item.Update(resource.Stock ?? item.Stock, resource.MinimumStock ?? item.MinimumStock); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        await db.SaveChangesAsync(cancellationToken);
        return Ok(item);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        if (!Owns(organizationId)) return Forbid();
        var item = await db.InventoryItems.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId, cancellationToken);
        if (item is null) return NotFound();
        db.InventoryItems.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private bool Owns(Guid organizationId) => Guid.TryParse(User.FindFirstValue("organization_id"), out var current) && current == organizationId;
}
