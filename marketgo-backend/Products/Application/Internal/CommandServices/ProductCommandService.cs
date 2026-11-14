using marketgo_backend.Products.Application.CommandServices;
using marketgo_backend.Products.Domain.Model;
using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Products.Domain.Model.Commands;
using marketgo_backend.Products.Domain.Model.Errors;
using marketgo_backend.Products.Domain.Repositories;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Suppliers.Interfaces.Acl;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Products.Application.Internal.CommandServices;
public sealed class ProductCommandService(IProductRepository products, ISuppliersContextFacade suppliers, IUnitOfWork unitOfWork) : IProductCommandService
{
    public async Task<Result<Product>> Handle(CreateProductCommand command, CancellationToken ct)
    {
        if (command.SupplierId is Guid supplierId && !await suppliers.ExistsActiveAsync(supplierId, ct))
            return Result<Product>.Failure(ProductErrors.SupplierUnavailable, ProductsError.SupplierUnavailable);
        try
        {
            var product = new Product(command.OrganizationId, command.Name, command.Category,
                command.Description, command.ExpirationDate, command.Quantity, command.Price, command.SupplierId);
            await products.AddAsync(product, ct);
            await unitOfWork.CompleteAsync(ct);
            return Result<Product>.Success(product);
        }
        catch (ArgumentException) { return Result<Product>.Failure(ProductErrors.InvalidProductData, ProductsError.InvalidProductData); }
    }
    public async Task<Result<Product>> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await products.FindByIdForOrganizationAsync(command.Id, command.OrganizationId, ct);
        if (product is null) return Result<Product>.Failure(ProductErrors.ProductNotFound, ProductsError.ProductNotFound);
        if (command.SupplierId is Guid supplierId && !await suppliers.ExistsActiveAsync(supplierId, ct))
            return Result<Product>.Failure(ProductErrors.SupplierUnavailable, ProductsError.SupplierUnavailable);
        try
        {
            product.Update(command.Name ?? product.Name, command.Category ?? product.Category,
                command.Description ?? product.Description, command.ExpirationDate ?? product.ExpirationDate,
                command.Quantity ?? product.Quantity, command.Price ?? product.Price,
                command.Available ?? product.Available, command.SupplierId ?? product.SupplierId);
            await unitOfWork.CompleteAsync(ct);
            return Result<Product>.Success(product);
        }
        catch (ArgumentException) { return Result<Product>.Failure(ProductErrors.InvalidProductData, ProductsError.InvalidProductData); }
    }
    public async Task<Result<Product>> Withdraw(Guid id, Guid organizationId, CancellationToken ct)
    {
        var product = await products.FindByIdForOrganizationAsync(id, organizationId, ct);
        if (product is null) return Result<Product>.Failure(ProductErrors.ProductNotFound, ProductsError.ProductNotFound);
        product.Withdraw();
        await unitOfWork.CompleteAsync(ct);
        return Result<Product>.Success(product);
    }
}
