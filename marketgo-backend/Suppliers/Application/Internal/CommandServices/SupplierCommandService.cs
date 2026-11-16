using marketgo_backend.Profiles.Interfaces.Acl;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Suppliers.Application.CommandServices;
using marketgo_backend.Suppliers.Domain.Model;
using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using marketgo_backend.Suppliers.Domain.Model.Commands;
using marketgo_backend.Suppliers.Domain.Model.Errors;
using marketgo_backend.Suppliers.Domain.Repositories;
namespace marketgo_backend.Suppliers.Application.Internal.CommandServices;
public sealed class SupplierCommandService(ISupplierRepository suppliers, IUnitOfWork unitOfWork, IProfilesContextFacade profiles) : ISupplierCommandService
{
    public async Task<Result<SupplierProfile>> Handle(CreateSupplierCommand command, CancellationToken ct)
    {
        if (command.SupplierOrganizationId is Guid supplierOrganizationId &&
            !await IsRegisteredSupplier(supplierOrganizationId, command.OrganizationId, ct))
            return Result<SupplierProfile>.Failure(SupplierErrors.SupplierOrganizationInvalid, SuppliersError.SupplierOrganizationInvalid);
        if (await suppliers.ExistsRucAsync(command.OrganizationId, command.Ruc, null, ct))
            return Result<SupplierProfile>.Failure(SupplierErrors.RucAlreadyExists, SuppliersError.RucAlreadyExists);
        try
        {
            var supplier = new SupplierProfile(command.OrganizationId, command.BusinessName, command.Ruc,
                command.Email, command.Phone, command.Address, command.Specialty, command.CoverageArea,
                command.SupplierOrganizationId);
            await suppliers.AddAsync(supplier, ct);
            await unitOfWork.CompleteAsync(ct);
            return Result<SupplierProfile>.Success(supplier);
        }
        catch (ArgumentException) { return Result<SupplierProfile>.Failure(SupplierErrors.InvalidData, SuppliersError.InvalidData); }
    }
    public async Task<Result<SupplierProfile>> Handle(UpdateSupplierCommand command, CancellationToken ct)
    {
        var supplier = await suppliers.FindByIdForOrganizationAsync(command.Id, command.OrganizationId, ct);
        if (supplier is null) return Result<SupplierProfile>.Failure(SupplierErrors.SupplierNotFound, SuppliersError.SupplierNotFound);
        if (command.SupplierOrganizationId is Guid linkedId &&
            !await IsRegisteredSupplier(linkedId, command.OrganizationId, ct))
            return Result<SupplierProfile>.Failure(SupplierErrors.SupplierOrganizationInvalid, SuppliersError.SupplierOrganizationInvalid);
        var ruc = command.Ruc ?? supplier.Ruc;
        if (await suppliers.ExistsRucAsync(command.OrganizationId, ruc, supplier.Id, ct))
            return Result<SupplierProfile>.Failure(SupplierErrors.RucAlreadyExists, SuppliersError.RucAlreadyExists);
        try
        {
            if (command.SupplierOrganizationId is Guid organizationToLink) supplier.LinkToOrganization(organizationToLink);
            supplier.Update(command.BusinessName ?? supplier.BusinessName, ruc, command.Email ?? supplier.Email,
                command.Phone ?? supplier.Phone, command.Address ?? supplier.Address,
                command.Specialty ?? supplier.Specialty, command.CoverageArea ?? supplier.CoverageArea,
                command.Status ?? supplier.Status);
            await unitOfWork.CompleteAsync(ct);
            return Result<SupplierProfile>.Success(supplier);
        }
        catch (ArgumentException) { return Result<SupplierProfile>.Failure(SupplierErrors.InvalidData, SuppliersError.InvalidData); }
    }
    private async Task<bool> IsRegisteredSupplier(Guid supplierOrganizationId, Guid minimarketId, CancellationToken ct) =>
        supplierOrganizationId != Guid.Empty && supplierOrganizationId != minimarketId &&
        await profiles.FetchByOrganization(supplierOrganizationId, ct) is { Type: "supplier" };
}
