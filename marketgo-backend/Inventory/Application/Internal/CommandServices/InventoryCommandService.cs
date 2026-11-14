using marketgo_backend.Inventory.Application.CommandServices;
using marketgo_backend.Inventory.Domain.Model;
using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Model.Commands;
using marketgo_backend.Inventory.Domain.Model.Errors;
using marketgo_backend.Inventory.Domain.Repositories;
using marketgo_backend.Products.Interfaces.Acl;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Inventory.Application.Internal.CommandServices;
public sealed class InventoryCommandService(IInventoryRepository inventory, IProductsContextFacade products,
    IUnitOfWork unitOfWork) : IInventoryCommandService
{
    public async Task<Result<InventoryItem>> Handle(CreateInventoryItemCommand command, CancellationToken ct)
    {
        var product = await products.FindAvailableAsync(command.ProductId, ct);
        if (product is null) return Result<InventoryItem>.Failure(InventoryErrors.ProductUnavailable, InventoryError.ProductUnavailable);
        try
        {
            var item = new InventoryItem(command.OrganizationId, product.Id, product.Name,
                command.Stock, command.MinimumStock, command.LotCode, command.ExpirationDate);
            await inventory.AddAsync(item, ct);
            await unitOfWork.CompleteAsync(ct);
            return Result<InventoryItem>.Success(item);
        }
        catch (ArgumentException) { return Result<InventoryItem>.Failure(InventoryErrors.InvalidData, InventoryError.InvalidData); }
    }
    public async Task<Result<InventoryItem>> Handle(UpdateInventoryItemCommand command, CancellationToken ct)
    {
        var item = await inventory.FindByIdForOrganizationAsync(command.Id, command.OrganizationId, ct);
        if (item is null) return Result<InventoryItem>.Failure(InventoryErrors.ItemNotFound, InventoryError.ItemNotFound);
        try
        {
            item.Update(command.Stock ?? item.Stock, command.MinimumStock ?? item.MinimumStock);
            await unitOfWork.CompleteAsync(ct);
            return Result<InventoryItem>.Success(item);
        }
        catch (ArgumentException) { return Result<InventoryItem>.Failure(InventoryErrors.InvalidData, InventoryError.InvalidData); }
    }
    public async Task<Result<InventoryItem>> Delete(Guid id, Guid organizationId, CancellationToken ct)
    {
        var item = await inventory.FindByIdForOrganizationAsync(id, organizationId, ct);
        if (item is null) return Result<InventoryItem>.Failure(InventoryErrors.ItemNotFound, InventoryError.ItemNotFound);
        inventory.Remove(item);
        await unitOfWork.CompleteAsync(ct);
        return Result<InventoryItem>.Success(item);
    }
    public async Task<Result<WasteRecord>> Handle(CreateWasteCommand command, CancellationToken ct)
    {
        var item = await inventory.FindByIdForOrganizationAsync(command.InventoryItemId, command.OrganizationId, ct);
        if (item is null) return Result<WasteRecord>.Failure(InventoryErrors.ItemNotFound, InventoryError.ItemNotFound);
        try
        {
            var waste = new WasteRecord(command.OrganizationId, item, command.Quantity, command.Reason);
            await inventory.AddWasteAsync(waste, ct);
            await unitOfWork.CompleteAsync(ct);
            return Result<WasteRecord>.Success(waste);
        }
        catch (ArgumentException) { return Result<WasteRecord>.Failure(InventoryErrors.InvalidData, InventoryError.InvalidData); }
    }
    public async Task<Result<DonationRecord>> Handle(CreateDonationCommand command, CancellationToken ct)
    {
        var item = await inventory.FindByIdForOrganizationAsync(command.InventoryItemId, command.OrganizationId, ct);
        if (item is null) return Result<DonationRecord>.Failure(InventoryErrors.ItemNotFound, InventoryError.ItemNotFound);
        try
        {
            var donation = new DonationRecord(command.OrganizationId, item, command.Quantity, command.Recipient);
            await inventory.AddDonationAsync(donation, ct);
            await unitOfWork.CompleteAsync(ct);
            return Result<DonationRecord>.Success(donation);
        }
        catch (ArgumentException) { return Result<DonationRecord>.Failure(InventoryErrors.InvalidData, InventoryError.InvalidData); }
    }
}
