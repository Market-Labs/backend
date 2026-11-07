using System.Security.Cryptography;
using System.Text;
using marketgo_backend.Iam.Application.Services;

namespace marketgo_backend.Iam.Infrastructure.Security;

public sealed class SessionTokenService : ISessionTokenService
{
    public string CreateToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
        .TrimEnd('=').Replace('+', '-').Replace('/', '_');

    public string HashToken(string token) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
