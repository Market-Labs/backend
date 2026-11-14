namespace marketgo_backend.Procurements.Domain.Model.Queries;
public sealed record GetPurchaseOrdersQuery(Guid OrganizationId, string AccountType);
