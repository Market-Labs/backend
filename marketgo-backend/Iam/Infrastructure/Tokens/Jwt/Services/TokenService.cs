using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using marketgo_backend.Iam.Application.Internal.OutboundServices;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Tokens.Jwt.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace marketgo_backend.Iam.Infrastructure.Tokens.Jwt.Services;

public sealed class TokenService(IOptions<TokenSettings> options) : ITokenService
{
    private readonly TokenSettings _settings = Validate(options.Value);
    private static TokenSettings Validate(TokenSettings settings) { settings.Validate(); return settings; }

    public string GenerateToken(User user)
    {
        if (user.Id.Value <= 0) throw new InvalidOperationException("User must be saved before issuing a token.");
        var now = DateTime.UtcNow;
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("organization_id", user.OrganizationId.ToString()),
            new Claim("account_type", user.AccountType),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_settings.Issuer, _settings.Audience, claims,
            notBefore: now, expires: now.AddHours(_settings.ExpirationHours), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<int?> ValidateToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true, ValidIssuer = _settings.Issuer,
                ValidateAudience = true, ValidAudience = _settings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)),
                ValidateLifetime = true, ClockSkew = TimeSpan.Zero
            }, out _);
            var subject = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Task.FromResult(int.TryParse(subject, out var id) ? (int?)id : null);
        }
        catch (SecurityTokenException) { return Task.FromResult<int?>(null); }
        catch (ArgumentException) { return Task.FromResult<int?>(null); }
    }
}
