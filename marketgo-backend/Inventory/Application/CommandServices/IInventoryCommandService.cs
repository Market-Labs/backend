using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Model.Commands;
using marketgo_backend.Shared.Application.Model;
namespace marketgo_backend.Inventory.Application.CommandServices;
public interface IInventoryCommandService
{
    Task<Result<InventoryItem>> Handle(CreateInventoryItemCommand command, CancellationToken cancellationToken);
    Task<Result<InventoryItem>> Handle(UpdateInventoryItemCommand command, CancellationToken cancellationToken);
    Task<Result<InventoryItem>> Delete(Guid id, Guid organizationId, CancellationToken cancellationToken);
    Task<Result<WasteRecord>> Handle(CreateWasteCommand command, CancellationToken cancellationToken);
    Task<Result<DonationRecord>> Handle(CreateDonationCommand command, CancellationToken cancellationToken);
}
