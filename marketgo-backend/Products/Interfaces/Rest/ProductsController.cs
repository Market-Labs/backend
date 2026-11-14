using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Products.Application.CommandServices;
using marketgo_backend.Products.Application.QueryServices;
using marketgo_backend.Products.Domain.Model;
using marketgo_backend.Products.Domain.Model.Commands;
using marketgo_backend.Products.Domain.Model.Queries;
using marketgo_backend.Products.Interfaces.Rest.Resources;
using marketgo_backend.Products.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
namespace marketgo_backend.Products.Interfaces.Rest;
[Authorize, ApiController, Route("api/v1/products"), Tags("Products")]
public sealed class ProductsController(IProductCommandService commands, IProductQueryService queries) : ControllerBase
{
    private Guid OrganizationId => ((User)HttpContext.Items["User"]!).OrganizationId;
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok((await queries.Handle(new GetProductsQuery(null), ct)).Select(ProductResourceFromEntityAssembler.ToResourceFromEntity));
    [HttpGet("/api/v1/suppliers/{organizationId:guid}/products")]
    public async Task<IActionResult> SupplierProducts(Guid organizationId, CancellationToken ct) =>
        organizationId != OrganizationId ? Forbid() :
        Ok((await queries.Handle(new GetProductsQuery(organizationId), ct)).Select(ProductResourceFromEntityAssembler.ToResourceFromEntity));
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var product = await queries.Handle(new GetProductByIdQuery(id), ct);
        return product is null ? NotFound() : Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(product));
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductResource resource, CancellationToken ct)
    {
        var result = await commands.Handle(new CreateProductCommand(OrganizationId, resource.Name,
            resource.Category, resource.Description, resource.ExpirationDate, resource.Quantity,
            resource.Price, resource.SupplierId), ct);
        return result.IsFailure ? BadRequest(new { error = result.Message }) :
            CreatedAtAction(nameof(Get), new { id = result.Value!.Id },
                ProductResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductResource resource, CancellationToken ct)
    {
        var result = await commands.Handle(new UpdateProductCommand(id, OrganizationId, resource.Name,
            resource.Category, resource.Description, resource.ExpirationDate, resource.Quantity,
            resource.Price, resource.Available, resource.SupplierId), ct);
        return result.IsFailure ? Error(result.Error) : Ok(ProductResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await commands.Withdraw(id, OrganizationId, ct);
        return result.IsFailure ? Error(result.Error) : NoContent();
    }
    private IActionResult Error(Enum? error) => error is ProductsError.ProductNotFound ? NotFound() : BadRequest();
}
