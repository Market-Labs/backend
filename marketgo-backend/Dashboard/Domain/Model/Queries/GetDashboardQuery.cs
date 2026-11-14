namespace marketgo_backend.Dashboard.Domain.Model.Queries;
public sealed record GetDashboardQuery(Guid OrganizationId, string AccountType);
