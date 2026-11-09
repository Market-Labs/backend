using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using marketgo_backend.Iam.Application.Services;
using marketgo_backend.Iam.Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace marketgo_backend.Iam.Infrastructure.Security;

public static class JwtSessionValidation
{
    public const string SessionIdClaim = "session_id";

    public static async Task ValidateAsync(TokenValidatedContext context)
    {
        var authorization = context.Request.Headers.Authorization.ToString();
        var token = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authorization[7..].Trim() : string.Empty;
        var subject = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrWhiteSpace(token) || !Guid.TryParse(subject, out var userId))
        {
            context.Fail("Invalid token subject.");
            return;
        }

        var services = context.HttpContext.RequestServices;
        var tokenService = services.GetRequiredService<IJwtTokenService>();
        var sessions = services.GetRequiredService<ISessionRepository>();
        var users = services.GetRequiredService<IUserRepository>();
        var clock = services.GetRequiredService<TimeProvider>();
        var session = await sessions.FindActiveByTokenHashAsync(
            tokenService.HashToken(token), clock.GetUtcNow(), context.HttpContext.RequestAborted);
        if (session is null || session.UserId != userId)
        {
            context.Fail("Session is invalid or revoked.");
            return;
        }

        var user = await users.FindByIdAsync(userId, context.HttpContext.RequestAborted);
        if (user is null || !user.IsActive)
        {
            context.Fail("Account is inactive.");
            return;
        }

        (context.Principal!.Identity as ClaimsIdentity)?.AddClaim(new Claim(SessionIdClaim, session.Id.ToString()));
    }
}
