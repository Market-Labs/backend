namespace marketgo_backend.Suppliers.Interfaces.Acl;
public interface ISuppliersContextFacade
{
    Task<bool> IsLinkedActiveAsync(Guid minimarketId, Guid supplierOrganizationId, CancellationToken cancellationToken);
    Task<bool> ExistsActiveAsync(Guid supplierId, CancellationToken cancellationToken);
    Task<int> CountActiveAsync(Guid organizationId, CancellationToken cancellationToken);
}
