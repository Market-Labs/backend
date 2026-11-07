using marketgo_backend.Iam.Domain.Model.Aggregates;

namespace marketgo_backend.Iam.Application.Services;

public interface IJwtTokenService
{
    string CreateToken(User user, DateTimeOffset issuedAt, DateTimeOffset expiresAt);
    string HashToken(string token);
}
