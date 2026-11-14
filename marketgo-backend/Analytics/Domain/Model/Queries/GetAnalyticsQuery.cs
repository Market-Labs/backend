namespace marketgo_backend.Analytics.Domain.Model.Queries;
public sealed record GetAnalyticsQuery(Guid OrganizationId, string AccountType);
