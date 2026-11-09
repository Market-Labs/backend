using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Suppliers.Interfaces.Rest;

public sealed record SupplierResource(
    [Required, StringLength(160)] string BusinessName,
    [Required, RegularExpression("^[0-9]{11}$")] string Ruc,
    [Required, EmailAddress] string Email,
    [Required] string Phone,
    [Required] string Address,
    [Required] string Specialty,
    [Required] string CoverageArea,
    string? Status);
public sealed record UpdateSupplierResource(string? BusinessName, string? Ruc, string? Email, string? Phone,
    string? Address, string? Specialty, string? CoverageArea, string? Status);

[ApiController]
[Authorize]
[Route("api/v1/suppliers")]
public sealed class SuppliersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken) =>
        Ok(await db.SupplierProfiles.AsNoTracking().Where(x => x.Status == "active").OrderBy(x => x.BusinessName).ToListAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var supplier = await db.SupplierProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SupplierResource resource, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue("organization_id"), out var organizationId)) return Forbid();
        var organization = await db.Organizations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == organizationId, cancellationToken);
        if (organization?.Kind != marketgo_backend.Profiles.Domain.Model.ValueObjects.OrganizationKind.Supplier)
            return BadRequest(new { error = "Only supplier organizations can create a supplier profile." });
        SupplierProfile supplier;
        try { supplier = new SupplierProfile(organizationId, resource.BusinessName, resource.Ruc, resource.Email,
            resource.Phone, resource.Address, resource.Specialty, resource.CoverageArea); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        if (await db.SupplierProfiles.AnyAsync(x => x.Ruc == supplier.Ruc, cancellationToken))
            return Conflict(new { error = "RUC is already registered." });
        db.SupplierProfiles.Add(supplier);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = supplier.Id }, supplier);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateSupplierResource resource, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue("organization_id"), out var organizationId)) return Forbid();
        var supplier = await db.SupplierProfiles.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId, cancellationToken);
        if (supplier is null) return NotFound();
        if (await db.SupplierProfiles.AnyAsync(x => x.Ruc == (resource.Ruc ?? supplier.Ruc) && x.Id != id, cancellationToken))
            return Conflict(new { error = "RUC is already registered." });
        try { supplier.Update(resource.BusinessName ?? supplier.BusinessName, resource.Ruc ?? supplier.Ruc,
            resource.Email ?? supplier.Email, resource.Phone ?? supplier.Phone, resource.Address ?? supplier.Address,
            resource.Specialty ?? supplier.Specialty, resource.CoverageArea ?? supplier.CoverageArea, resource.Status ?? supplier.Status); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        await db.SaveChangesAsync(cancellationToken);
        return Ok(supplier);
    }
}
