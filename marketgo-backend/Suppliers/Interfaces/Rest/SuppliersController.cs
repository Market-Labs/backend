using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Suppliers.Application.CommandServices;
using marketgo_backend.Suppliers.Application.QueryServices;
using marketgo_backend.Suppliers.Domain.Model;
using marketgo_backend.Suppliers.Domain.Model.Commands;
using marketgo_backend.Suppliers.Domain.Model.Queries;
using marketgo_backend.Suppliers.Interfaces.Rest.Resources;
using marketgo_backend.Suppliers.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
namespace marketgo_backend.Suppliers.Interfaces.Rest;
[Authorize, ApiController, Route("api/v1/suppliers"), Tags("Suppliers")]
public sealed class SuppliersController(ISupplierCommandService commands, ISupplierQueryService queries) : ControllerBase
{
    private Guid OrganizationId => ((User)HttpContext.Items["User"]!).OrganizationId;
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok((await queries.Handle(new GetSuppliersQuery(OrganizationId), ct))
            .Select(SupplierResourceFromEntityAssembler.ToResourceFromEntity));
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var supplier = await queries.Handle(new GetSupplierByIdQuery(id, OrganizationId), ct);
        return supplier is null ? NotFound() : Ok(SupplierResourceFromEntityAssembler.ToResourceFromEntity(supplier));
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateSupplierResource resource, CancellationToken ct)
    {
        var result = await commands.Handle(new CreateSupplierCommand(OrganizationId, resource.BusinessName,
            resource.Ruc, resource.Email, resource.Phone, resource.Address, resource.Specialty, resource.CoverageArea), ct);
        if (result.IsFailure) return Error(result.Error);
        var supplier = result.Value!;
        return CreatedAtAction(nameof(Get), new { id = supplier.Id }, SupplierResourceFromEntityAssembler.ToResourceFromEntity(supplier));
    }
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateSupplierResource resource, CancellationToken ct)
    {
        var result = await commands.Handle(new UpdateSupplierCommand(id, OrganizationId, resource.BusinessName,
            resource.Ruc, resource.Email, resource.Phone, resource.Address, resource.Specialty,
            resource.CoverageArea, resource.Status), ct);
        return result.IsFailure ? Error(result.Error) : Ok(SupplierResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
    private IActionResult Error(Enum? error) => error switch
    {
        SuppliersError.SupplierNotFound => NotFound(),
        SuppliersError.RucAlreadyExists => Conflict(new { error = "RUC already registered." }),
        _ => BadRequest(new { error = "Invalid supplier data." })
    };
}
