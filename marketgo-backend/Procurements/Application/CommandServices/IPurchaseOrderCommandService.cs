using marketgo_backend.Procurements.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Domain.Model.Commands;
using marketgo_backend.Shared.Application.Model;
namespace marketgo_backend.Procurements.Application.CommandServices;
public interface IPurchaseOrderCommandService
{
    Task<Result<PurchaseOrder>> Handle(CreatePurchaseOrderCommand command, CancellationToken cancellationToken);
    Task<Result<PurchaseOrder>> Handle(ReviewPurchaseOrderCommand command, CancellationToken cancellationToken);
}
