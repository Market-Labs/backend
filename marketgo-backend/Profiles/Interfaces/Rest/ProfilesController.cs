using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using marketgo_backend.Profiles.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace marketgo_backend.Profiles.Interfaces.Rest;

[ApiController]
[Authorize]
[Route("api/v1/profiles")]
public sealed class ProfilesController(IProfileService profiles) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        if (!TryIdentity(out var organizationId, out var userId)) return Unauthorized();
        var profile = await profiles.GetOwnAsync(organizationId, userId, cancellationToken);
        return Ok(profile is null ? Array.Empty<ProfileResource>() : [profile]);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        if (!TryIdentity(out var organizationId, out var userId)) return Unauthorized();
        if (id != organizationId) return NotFound();
        var profile = await profiles.GetOwnAsync(organizationId, userId, cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        if (!TryIdentity(out var organizationId, out var userId)) return Unauthorized();
        var result = await profiles.UpdateAsync(organizationId, userId, id, command, cancellationToken);
        return result.Fold<IActionResult>(Ok, error => error switch
        {
            ProfileError.NotFound => NotFound(),
            ProfileError.InvalidInput => BadRequest(new { error = "Invalid profile data." }),
            _ => Problem(statusCode: 500)
        });
    }

    private bool TryIdentity(out Guid organizationId, out Guid userId)
    {
        var validOrganization = Guid.TryParse(User.FindFirstValue("organization_id"), out organizationId);
        var validUser = Guid.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out userId);
        return validOrganization && validUser;
    }
}


