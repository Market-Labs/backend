using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Iam.Interfaces.Rest;

public sealed record UpdateUserResource(string? FullName);
public sealed record ManagedUserResource(Guid Id, Guid OrganizationId, string FullName, string Email,
    string Role, bool IsActive, DateTimeOffset? LastSignedInAt);

[ApiController, Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/users")]
[Route("api/v1/suppliers/{organizationId:guid}/users")]
public sealed class UsersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var users = await db.Users.AsNoTracking().Where(x => x.OrganizationId == organizationId)
            .OrderBy(x => x.FullName).Select(x => new ManagedUserResource(x.Id, x.OrganizationId, x.FullName,
                x.Email, x.Role, x.IsActive, x.LastSignedInAt)).ToListAsync(ct);
        return Ok(users);
    }
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid organizationId, Guid id, UpdateUserResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        if (!Guid.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var currentUser) || currentUser != id) return Forbid();
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == organizationId, ct);
        if (user is null) return NotFound();
        try { user.UpdateName(resource.FullName); }
        catch (ArgumentException error) { return BadRequest(new { error = error.Message }); }
        await db.SaveChangesAsync(ct);
        return Ok(new ManagedUserResource(user.Id, user.OrganizationId, user.FullName, user.Email,
            user.Role, user.IsActive, user.LastSignedInAt));
    }
    private bool Owns(Guid id) => Guid.TryParse(User.FindFirstValue("organization_id"), out var current) && current == id;
}
