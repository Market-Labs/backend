using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Profiles.Interfaces.Rest;

public sealed record ClientContactInput(string BusinessName, string Email, string Phone, string Address, string District);

[Authorize, ApiController, Route("api/v1/suppliers/{organizationId:guid}/clients"), Tags("Profiles")]
public sealed class ClientContactsController(AppDbContext context, IUnitOfWork unitOfWork) : ControllerBase
{
    private User Caller => (User)HttpContext.Items["User"]!;
    private bool CanAccess(Guid organizationId) => Caller.AccountType == "supplier" && Caller.OrganizationId == organizationId;

    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct)
    {
        if (!CanAccess(organizationId)) return StatusCode(StatusCodes.Status403Forbidden);
        return Ok(await context.Set<ClientContact>().AsNoTracking()
            .Where(contact => contact.SupplierOrganizationId == organizationId)
            .OrderBy(contact => contact.BusinessName).ToListAsync(ct));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, ClientContactInput input, CancellationToken ct)
    {
        if (!CanAccess(organizationId)) return StatusCode(StatusCodes.Status403Forbidden);
        try
        {
            var contact = new ClientContact(organizationId, input.BusinessName, input.Email,
                input.Phone, input.Address, input.District);
            await context.Set<ClientContact>().AddAsync(contact, ct);
            await unitOfWork.CompleteAsync(ct);
            return Created($"/api/v1/suppliers/{organizationId}/clients/{contact.Id}", contact);
        }
        catch (ArgumentException) { return BadRequest(new { error = "Invalid client contact." }); }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid organizationId, Guid id, CancellationToken ct)
    {
        if (!CanAccess(organizationId)) return StatusCode(StatusCodes.Status403Forbidden);
        var contact = await context.Set<ClientContact>().AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id && item.SupplierOrganizationId == organizationId, ct);
        return contact is null ? NotFound() : Ok(contact);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid organizationId, Guid id, ClientContactInput input, CancellationToken ct)
    {
        if (!CanAccess(organizationId)) return StatusCode(StatusCodes.Status403Forbidden);
        var contact = await context.Set<ClientContact>()
            .FirstOrDefaultAsync(item => item.Id == id && item.SupplierOrganizationId == organizationId, ct);
        if (contact is null) return NotFound();
        try
        {
            contact.Update(input.BusinessName, input.Email, input.Phone, input.Address, input.District);
            await unitOfWork.CompleteAsync(ct);
            return Ok(contact);
        }
        catch (ArgumentException) { return BadRequest(new { error = "Invalid client contact." }); }
    }
}