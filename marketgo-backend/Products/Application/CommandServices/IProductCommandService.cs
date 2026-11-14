using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Products.Domain.Model.Commands;
using marketgo_backend.Shared.Application.Model;
namespace marketgo_backend.Products.Application.CommandServices;
public interface IProductCommandService
{
    Task<Result<Product>> Handle(CreateProductCommand command, CancellationToken cancellationToken);
    Task<Result<Product>> Handle(UpdateProductCommand command, CancellationToken cancellationToken);
    Task<Result<Product>> Withdraw(Guid id, Guid organizationId, CancellationToken cancellationToken);
}
