namespace marketgo_backend.Iam.Infrastructure.Tokens.Jwt.Configuration;

public sealed class TokenSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "marketgo-backend";
    public string Audience { get; set; } = "marketgo-frontend";
    public int ExpirationHours { get; set; } = 12;

    public void Validate()
    {
        if (System.Text.Encoding.UTF8.GetByteCount(Secret) < 32)
            throw new InvalidOperationException("TokenSettings:Secret must contain at least 32 UTF-8 bytes.");
        if (string.IsNullOrWhiteSpace(Issuer) || string.IsNullOrWhiteSpace(Audience) ||
            ExpirationHours is < 1 or > 24)
            throw new InvalidOperationException("Invalid token settings.");
    }
}
