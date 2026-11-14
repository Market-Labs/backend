namespace marketgo_backend.Iam.Domain.Model.Queries;

// The Learning Center query name is retained; MarketGo authenticates with email.
public sealed record GetUserByUsernameQuery(string Email);
