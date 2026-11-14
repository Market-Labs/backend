using marketgo_backend.Requisition.Application.CommandServices;
using marketgo_backend.Suppliers.Interfaces.Acl;
using marketgo_backend.Requisition.Domain.Model;
using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Model.Commands;
using marketgo_backend.Requisition.Domain.Model.Errors;
using marketgo_backend.Requisition.Domain.Repositories;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Requisition.Application.Internal.CommandServices;
public sealed class SupplyRequestCommandService(ISupplyRequestRepository requests, IUnitOfWork unitOfWork,
    TimeProvider clock, ISuppliersContextFacade suppliers) : ISupplyRequestCommandService
{
    public async Task<Result<SupplyRequest>> Handle(CreateSupplyRequestCommand command, CancellationToken ct)
    {
        if (!await suppliers.IsLinkedActiveAsync(command.MinimarketId, command.SupplierOrganizationId, ct))
            return Result<SupplyRequest>.Failure(RequisitionErrors.InvalidSupplierOrganization, RequisitionError.InvalidSupplierOrganization);
        try
        {
            var request = new SupplyRequest(command.MinimarketId, command.SupplierOrganizationId, command.RequesterId,
                command.Requester, command.Supplier, command.Reason, command.Items);
            await requests.AddAsync(request, ct);
            await unitOfWork.CompleteAsync(ct);
            return Result<SupplyRequest>.Success(request);
        }
        catch (ArgumentException) { return Result<SupplyRequest>.Failure(RequisitionErrors.InvalidData, RequisitionError.InvalidData); }
    }
    public async Task<Result<SupplyRequest>> Handle(ReviewSupplyRequestCommand command, CancellationToken ct)
    {
        var request = await requests.FindAsync(command.Id, ct);
        if (request is null || request.SupplierId != command.SupplierId)
            return Result<SupplyRequest>.Failure(RequisitionErrors.NotFound, RequisitionError.NotFound);
        try
        {
            request.Review(command.Status, command.RejectionReason, clock.GetUtcNow());
            await unitOfWork.CompleteAsync(ct);
            return Result<SupplyRequest>.Success(request);
        }
        catch (ArgumentException) { return Result<SupplyRequest>.Failure(RequisitionErrors.InvalidData, RequisitionError.InvalidData); }
        catch (InvalidOperationException) { return Result<SupplyRequest>.Failure(RequisitionErrors.InvalidTransition, RequisitionError.InvalidTransition); }
    }
}
