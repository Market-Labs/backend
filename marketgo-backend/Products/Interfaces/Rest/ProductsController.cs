using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Products.Interfaces.Rest;

public sealed record CreateProductResource(
    [Required, StringLength(160, MinimumLength = 1)] string Name,
    [Required, StringLength(80, MinimumLength = 1)] string Category,
    [Required, StringLength(1000, MinimumLength = 1)] string Description,
    DateOnly ExpirationDate,
    [Range(0, int.MaxValue)] int Quantity,
    [Range(typeof(decimal), "0.01", "9999999999.99")] decimal Price,
    Guid? SupplierId);

public sealed record UpdateProductResource(string? Description, decimal? Price, int? Quantity, bool? Available);

[ApiController]
[Authorize]
[Route("api/v1/products")]
public sealed class ProductsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken) =>
        Ok(await db.Products.AsNoTracking().Where(x => x.Available).OrderBy(x => x.Name).ToListAsync(cancellationToken));

    [HttpGet("/api/v1/suppliers/{organizationId:guid}/products")]
    public async Task<IActionResult> SupplierProducts(Guid organizationId, CancellationToken cancellationToken)
    {
        if (!TryOwner(out var current) || current != organizationId) return Forbid();
        return Ok(await db.Products.AsNoTracking().Where(x => x.OrganizationId == organizationId && x.Available)
            .OrderBy(x => x.Name).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.Available, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductResource resource, CancellationToken cancellationToken)
    {
        if (!TryOwner(out var organizationId)) return Forbid();
        Product product;
        try
        {
            product = new Product(organizationId, resource.Name, resource.Category, resource.Description,
                resource.ExpirationDate, resource.Quantity, resource.Price, resource.SupplierId);
        }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductResource resource, CancellationToken cancellationToken)
    {
        if (!TryOwner(out var organizationId)) return Forbid();
        var product = await db.Products.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId, cancellationToken);
        if (product is null) return NotFound();
        try { product.Update(resource.Description ?? product.Description, resource.Price ?? product.Price,
            resource.Quantity ?? product.Quantity, resource.Available ?? product.Available); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        await db.SaveChangesAsync(cancellationToken);
        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!TryOwner(out var organizationId)) return Forbid();
        var product = await db.Products.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId, cancellationToken);
        if (product is null) return NotFound();
        product.Update(product.Description, product.Price, product.Quantity, false);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private bool TryOwner(out Guid organizationId) =>
        Guid.TryParse(User.FindFirstValue("organization_id"), out organizationId);
}
