using marketgo_backend.Iam.Domain.Model.Aggregates;

namespace marketgo_backend.Iam.Application.Internal.OutboundServices;

public interface ITokenService
{
    string GenerateToken(User user);
    Task<int?> ValidateToken(string token);
}
