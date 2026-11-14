using marketgo_backend.Shared.Domain.Model.Events;
namespace marketgo_backend.Suppliers.Domain.Model.Events;
public sealed record SupplierRegisteredEvent(Guid SupplierId, Guid OrganizationId) : IEvent;
