namespace marketgo_backend.Requisition.Domain.Model.Queries;
public sealed record GetSupplyRequestsQuery(Guid OrganizationId, string AccountType);
