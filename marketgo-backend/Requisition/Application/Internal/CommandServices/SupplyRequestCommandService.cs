using marketgo_backend.Requisition.Application.CommandServices;
using marketgo_backend.Profiles.Interfaces.Acl;
using marketgo_backend.Requisition.Domain.Model;
using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Model.Commands;
using marketgo_backend.Requisition.Domain.Model.Errors;
using marketgo_backend.Requisition.Domain.Repositories;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Requisition.Application.Internal.CommandServices;
public sealed class SupplyRequestCommandService(ISupplyRequestRepository requests, IUnitOfWork unitOfWork,
    TimeProvider clock, IProfilesContextFacade profiles) : ISupplyRequestCommandService
{
    public async Task<Result<SupplyRequest>> Handle(CreateSupplyRequestCommand command, CancellationToken ct)
    {
        var supplier = await profiles.FetchByOrganization(command.SupplierOrganizationId, ct);
        if (supplier?.Type != "provider")
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
