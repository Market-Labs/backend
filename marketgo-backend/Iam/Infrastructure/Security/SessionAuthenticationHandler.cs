using System.Security.Claims;
using System.Text.Encodings.Web;
using marketgo_backend.Iam.Application.Services;
using marketgo_backend.Iam.Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace marketgo_backend.Iam.Infrastructure.Security;

public sealed class SessionAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISessionRepository sessions,
    IUserRepository users,
    ISessionTokenService tokens,
    TimeProvider clock) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "MarketGoSession";
    public const string SessionIdClaim = "session_id";
    public const string OrganizationIdClaim = "organization_id";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorization)) return AuthenticateResult.NoResult();
        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.Fail("Expected a Bearer token.");

        var token = authorization[7..].Trim();
        if (token.Length != 43 || token.Any(character => !char.IsAsciiLetterOrDigit(character) && character is not '-' and not '_'))
            return AuthenticateResult.Fail("Invalid session token.");

        var session = await sessions.FindActiveByTokenHashAsync(tokens.HashToken(token), clock.GetUtcNow(), Context.RequestAborted);
        if (session is null) return AuthenticateResult.Fail("Invalid or expired session.");

        var user = await users.FindByIdAsync(session.UserId, Context.RequestAborted);
        if (user is null || !user.IsActive) return AuthenticateResult.Fail("Inactive account.");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(OrganizationIdClaim, user.OrganizationId.ToString()),
            new Claim(SessionIdClaim, session.Id.ToString())
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}
