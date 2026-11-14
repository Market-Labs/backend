using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Model.Commands;
using marketgo_backend.Shared.Application.Model;
namespace marketgo_backend.Requisition.Application.CommandServices;
public interface ISupplyRequestCommandService
{
    Task<Result<SupplyRequest>> Handle(CreateSupplyRequestCommand command, CancellationToken cancellationToken);
    Task<Result<SupplyRequest>> Handle(ReviewSupplyRequestCommand command, CancellationToken cancellationToken);
}
