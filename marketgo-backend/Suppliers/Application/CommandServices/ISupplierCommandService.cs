using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using marketgo_backend.Suppliers.Domain.Model.Commands;
namespace marketgo_backend.Suppliers.Application.CommandServices;
public interface ISupplierCommandService
{
    Task<Result<SupplierProfile>> Handle(CreateSupplierCommand command, CancellationToken cancellationToken);
    Task<Result<SupplierProfile>> Handle(UpdateSupplierCommand command, CancellationToken cancellationToken);
}
