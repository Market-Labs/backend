namespace marketgo_backend.Suppliers.Interfaces.Acl;
public interface ISuppliersContextFacade
{
    Task<bool> ExistsActiveAsync(Guid supplierId, CancellationToken cancellationToken);
    Task<int> CountActiveAsync(Guid organizationId, CancellationToken cancellationToken);
}
