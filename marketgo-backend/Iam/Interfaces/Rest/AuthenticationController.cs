using System.Security.Claims;
using marketgo_backend.Iam.Application.Commands;
using marketgo_backend.Iam.Application.Errors;
using marketgo_backend.Iam.Application.Services;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Iam.Infrastructure.Security;
using marketgo_backend.Iam.Interfaces.Rest.Resources;
using marketgo_backend.Iam.Interfaces.Rest.Transform;
using marketgo_backend.Profiles.Domain.Model.ValueObjects;
using marketgo_backend.Profiles.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace marketgo_backend.Iam.Interfaces.Rest;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthenticationController(
    IAuthenticationService authentication,
    IUserRepository users,
    IOrganizationRepository organizations) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("sign-up")]
    [ProducesResponseType<AuthenticatedUserResource>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SignUp(SignUpResource resource, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<OrganizationKind>(resource.AccountType, true, out var kind) || !Enum.IsDefined(kind))
            return Problem(title: "Invalid account type.", statusCode: StatusCodes.Status400BadRequest);

        var command = new SignUpCommand(resource.FullName, resource.Email, resource.Password,
            resource.OrganizationName, kind, resource.AcceptedTerms);
        var result = await authentication.SignUpAsync(command, cancellationToken);
        return result.Fold<IActionResult>(
            session => Created("/api/v1/auth/me", AuthResourceAssembler.ToResource(session)),
            ToError);
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    [ProducesResponseType<AuthenticatedUserResource>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignIn(SignInResource resource, CancellationToken cancellationToken)
    {
        var result = await authentication.SignInAsync(
            new SignInCommand(resource.Email, resource.Password), cancellationToken);
        return result.Fold<IActionResult>(
            session => Ok(AuthResourceAssembler.ToResource(session)),
            ToError);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<UserResource>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub), out var userId))
            return Unauthorized();
        var user = await users.FindByIdAsync(userId, cancellationToken);
        if (user is null) return Unauthorized();
        var organization = await organizations.FindByIdAsync(user.OrganizationId, cancellationToken);
        if (organization is null) return NotFound();
        return Ok(AuthResourceAssembler.ToResource(user, organization));
    }

    [Authorize]
    [HttpPost("sign-out")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignOut(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(JwtSessionValidation.SessionIdClaim), out var sessionId))
            return Unauthorized();
        await authentication.RevokeSessionAsync(sessionId, cancellationToken);
        return NoContent();
    }

    private IActionResult ToError(AuthenticationError error) => error switch
    {
        AuthenticationError.InvalidInput => Problem(title: "Invalid registration data.", statusCode: StatusCodes.Status400BadRequest),
        AuthenticationError.EmailAlreadyRegistered => Problem(title: "Email is already registered.", statusCode: StatusCodes.Status409Conflict),
        AuthenticationError.InvalidCredentials => Problem(title: "Invalid email or password.", statusCode: StatusCodes.Status401Unauthorized),
        _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
    };
}
