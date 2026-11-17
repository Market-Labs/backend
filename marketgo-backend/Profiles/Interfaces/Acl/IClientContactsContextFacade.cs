using marketgo_backend.Profiles.Domain.Model.Aggregates;

namespace marketgo_backend.Profiles.Interfaces.Acl;

public interface IClientContactsContextFacade
{
    Task<ClientContact?> FetchForSupplierAsync(Guid contactId, Guid supplierOrganizationId,
        CancellationToken cancellationToken);
}
