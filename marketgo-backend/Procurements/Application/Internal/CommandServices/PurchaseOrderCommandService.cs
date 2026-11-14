using marketgo_backend.Procurements.Application.CommandServices;
using marketgo_backend.Procurements.Domain.Model;
using marketgo_backend.Procurements.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Domain.Model.Commands;
using marketgo_backend.Procurements.Domain.Model.Errors;
using marketgo_backend.Procurements.Domain.Repositories;
using marketgo_backend.Requisition.Interfaces.Acl;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Procurements.Application.Internal.CommandServices;
public sealed class PurchaseOrderCommandService(IPurchaseOrderRepository orders, IRequisitionContextFacade requests,
    IUnitOfWork unitOfWork, TimeProvider clock) : IPurchaseOrderCommandService
{
    public async Task<Result<PurchaseOrder>> Handle(CreatePurchaseOrderCommand command, CancellationToken ct)
    {
        var request = await requests.FindAcceptedAsync(command.SupplyRequestId, command.SupplierId, ct);
        if (request is null) return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidRequest, ProcurementsError.InvalidRequest);
        if (command.Items.Count != request.Items.Count ||
            command.Items.Any(item => !request.Items.Any(expected =>
                expected.ProductName == item.ProductName && item.Quantity <= expected.Quantity)))
            return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidData, ProcurementsError.InvalidData);
        try
        {
            var order = new PurchaseOrder(command.SupplierId, request.MinimarketId, request.Id,
                command.Supplier, command.Minimarket, command.ShippingDate, command.Observations, command.Items);
            requests.LinkOrder(request, order.Id);
            await orders.AddAsync(order, ct);
            await unitOfWork.CompleteAsync(ct);
            return Result<PurchaseOrder>.Success(order);
        }
        catch (ArgumentException) { return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidData, ProcurementsError.InvalidData); }
    }
    public async Task<Result<PurchaseOrder>> Handle(ReviewPurchaseOrderCommand command, CancellationToken ct)
    {
        var order = await orders.FindAsync(command.Id, ct);
        if (order is null || order.MinimarketId != command.MinimarketId)
            return Result<PurchaseOrder>.Failure(ProcurementsErrors.NotFound, ProcurementsError.NotFound);
        try
        {
            order.ReviewReception(command.Status, command.RejectionReason, command.ReviewerId, clock.GetUtcNow());
            await unitOfWork.CompleteAsync(ct);
            return Result<PurchaseOrder>.Success(order);
        }
        catch (ArgumentException) { return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidData, ProcurementsError.InvalidData); }
        catch (InvalidOperationException) { return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidTransition, ProcurementsError.InvalidTransition); }
    }
}
