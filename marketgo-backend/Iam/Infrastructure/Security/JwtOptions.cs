using System.Text;

namespace marketgo_backend.Iam.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Issuer) || string.IsNullOrWhiteSpace(Audience) ||
            Encoding.UTF8.GetByteCount(SigningKey) < 32)
            throw new InvalidOperationException("Set Jwt:Issuer, Jwt:Audience and a Jwt:SigningKey of at least 32 bytes.");
    }
}
