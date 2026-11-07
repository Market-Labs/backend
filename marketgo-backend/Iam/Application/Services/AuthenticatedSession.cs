using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Organizations.Domain.Model.Aggregates;

namespace marketgo_backend.Iam.Application.Services;

public sealed record AuthenticatedSession(
    User User,
    Organization Organization,
    string Token,
    DateTimeOffset ExpiresAt);
