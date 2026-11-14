namespace marketgo_backend.Inventory.Domain.Model.Queries;
public sealed record GetInventoryQuery(Guid OrganizationId, string? Search = null, int? ExpiringWithinDays = null);
