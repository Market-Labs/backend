using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Interfaces.Acl;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Profiles.Application.Acl;

public sealed class ClientContactsContextFacade(AppDbContext context) : IClientContactsContextFacade
{
    public Task<ClientContact?> FetchForSupplierAsync(Guid contactId, Guid supplierOrganizationId,
        CancellationToken cancellationToken) =>
        context.Set<ClientContact>().AsNoTracking().FirstOrDefaultAsync(contact =>
            contact.Id == contactId && contact.SupplierOrganizationId == supplierOrganizationId, cancellationToken);
}
