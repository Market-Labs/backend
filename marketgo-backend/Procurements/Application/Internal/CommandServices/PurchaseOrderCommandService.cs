using marketgo_backend.Inventory.Interfaces.Acl;
using marketgo_backend.Procurements.Application.CommandServices;
using marketgo_backend.Procurements.Domain.Model;
using marketgo_backend.Procurements.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Domain.Model.Commands;
using marketgo_backend.Procurements.Domain.Model.Errors;
using marketgo_backend.Procurements.Domain.Repositories;
using marketgo_backend.Products.Domain.Repositories;
using marketgo_backend.Profiles.Interfaces.Acl;
using marketgo_backend.Requisition.Interfaces.Acl;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Procurements.Application.Internal.CommandServices;
public sealed class PurchaseOrderCommandService(IPurchaseOrderRepository orders, IRequisitionContextFacade requests,
    IInventoryReceiptFacade inventory, IUnitOfWork unitOfWork, TimeProvider clock,
    IProfilesContextFacade profiles, IClientContactsContextFacade contacts,
    IProductRepository products) : IPurchaseOrderCommandService
{
    public async Task<Result<PurchaseOrder>> Handle(CreatePurchaseOrderCommand command, CancellationToken ct)
    {
        var request = command.SupplyRequestId.HasValue
            ? await requests.FindAcceptedAsync(command.SupplyRequestId.Value, command.SupplierId, ct)
            : null;
        if (command.SupplyRequestId.HasValue && request is null)
            return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidRequest, ProcurementsError.InvalidRequest);
        if (request is not null && (command.Items.Count != request.Items.Count ||
            command.Items.Any(item => !request.Items.Any(expected =>
                expected.ProductName == item.ProductName && item.Quantity <= expected.Quantity))))
            return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidData, ProcurementsError.InvalidData);
        if ((request is not null && command.ClientContactId.HasValue) ||
            (request is null && command.MinimarketId.HasValue == command.ClientContactId.HasValue))
            return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidData, ProcurementsError.InvalidData);
        var minimarketId = request?.MinimarketId ?? command.MinimarketId;
        if (minimarketId == Guid.Empty || minimarketId == command.SupplierId || command.ClientContactId == Guid.Empty)
            return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidData, ProcurementsError.InvalidData);
        var minimarket = minimarketId.HasValue
            ? await profiles.FetchByOrganization(minimarketId.Value, ct) : null;
        var contact = command.ClientContactId.HasValue
            ? await contacts.FetchForSupplierAsync(command.ClientContactId.Value, command.SupplierId, ct) : null;
        var supplier = await profiles.FetchByOrganization(command.SupplierId, ct);
        if ((minimarketId.HasValue && (minimarket?.Type != "minimarket" || !minimarket.IsListed)) ||
            (command.ClientContactId.HasValue && contact is null) || supplier?.Type != "provider")
            return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidData, ProcurementsError.InvalidData);
        if (request is null)
        {
            var catalog = await products.ListAvailableAsync(command.SupplierId, ct);
            if (command.Items is null || command.Items.Count is < 1 or > 100 ||
                command.Items.Select(item => item.ProductName).Distinct(StringComparer.OrdinalIgnoreCase).Count() != command.Items.Count ||
                command.Items.Any(item =>
                !catalog.Any(product => product.Name == item.ProductName && product.Price == item.UnitPrice)))
                return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidData, ProcurementsError.InvalidData);
        }
        try
        {
            var order = new PurchaseOrder(command.SupplierId, minimarketId, contact?.Id, request?.Id,
                supplier.BusinessName, minimarket?.BusinessName ?? contact!.BusinessName,
                command.ShippingDate, command.Observations, command.Items);
            if (request is not null) requests.LinkOrder(request, order.Id);
            await orders.AddAsync(order, ct);
            await unitOfWork.CompleteAsync(ct);
            return Result<PurchaseOrder>.Success(order);
        }
        catch (ArgumentException) { return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidData, ProcurementsError.InvalidData); }
    }
    public async Task<Result<PurchaseOrder>> Handle(ReviewPurchaseOrderCommand command, CancellationToken ct)
    {
        if (command.Status is not ("received" or "rejected"))
            return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidTransition, ProcurementsError.InvalidTransition);
        try
        {
            return await unitOfWork.ExecuteInTransactionAsync(async token =>
            {
                var order = await orders.FindForReceiptAsync(command.Id, command.MinimarketId, token);
                if (order is null)
                    return Result<PurchaseOrder>.Failure(ProcurementsErrors.NotFound, ProcurementsError.NotFound);
                if (order.Status != "pending-reception")
                    return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidTransition, ProcurementsError.InvalidTransition);
                if (command.Status == "received")
                {
                    if (command.ReceivedLines is null || command.ReceivedLines.Any(x =>
                        x is null || x.Lots is null || x.Lots.Any(lot => lot is null)))
                        throw new ArgumentException("Received lines and lots are required.");
                    await inventory.ReceiveAsync(order.MinimarketId!.Value, order.SupplierId, order.Id,
                        order.Items.Select(x => new InventoryReceiptOrderItem(x.ProductName, x.Quantity, x.UnitPrice)).ToList(),
                        command.ReceivedLines.Select(x => new InventoryReceiptLine(x.LineIndex, x.ProductId,
                            x.Category, x.Description, x.Lots?.Select(lot =>
                                new InventoryReceiptLot(lot.LotCode, lot.ExpirationDate, lot.Quantity)).ToList()!)).ToList(),
                        token);
                }
                else if (command.ReceivedLines is { Count: > 0 })
                    throw new ArgumentException("Rejected orders cannot contain received lines.");
                order.ReviewReception(command.Status, command.RejectionReason, command.ReviewerId, clock.GetUtcNow());
                return Result<PurchaseOrder>.Success(order);
            }, ct);
        }
        catch (ArgumentException) { return Result<PurchaseOrder>.Failure(ProcurementsErrors.InvalidData, ProcurementsError.InvalidData); }
    }
}
