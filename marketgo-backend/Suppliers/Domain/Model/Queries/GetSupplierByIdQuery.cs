namespace marketgo_backend.Suppliers.Domain.Model.Queries;
public sealed record GetSupplierByIdQuery(Guid Id, Guid OrganizationId);
