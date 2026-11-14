using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.ValueObjects;
using marketgo_backend.Products.Application.CommandServices;
using marketgo_backend.Products.Application.QueryServices;
using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Products.Domain.Model.Commands;
using marketgo_backend.Products.Domain.Model.Queries;
using marketgo_backend.Products.Interfaces.Rest;
using marketgo_backend.Products.Interfaces.Rest.Resources;
using marketgo_backend.Profiles.Application.CommandServices;
using marketgo_backend.Profiles.Application.QueryServices;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Commands;
using marketgo_backend.Profiles.Domain.Model.Queries;
using marketgo_backend.Profiles.Domain.Model.ValueObjects;
using marketgo_backend.Profiles.Interfaces.Rest;
using marketgo_backend.Profiles.Interfaces.Rest.Resources;
using marketgo_backend.Requisition.Application.CommandServices;
using marketgo_backend.Requisition.Application.Internal.CommandServices;
using marketgo_backend.Requisition.Application.QueryServices;
using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Model.Commands;
using marketgo_backend.Requisition.Domain.Model.Queries;
using marketgo_backend.Requisition.Domain.Model.ValueObjects;
using marketgo_backend.Requisition.Domain.Repositories;
using marketgo_backend.Requisition.Interfaces.Rest;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using marketgo_backend.Suppliers.Interfaces.Acl;
using marketgo_backend.Suppliers.Interfaces.Rest.Transform;
using marketgo_backend.Suppliers.Interfaces.Rest;
using marketgo_backend.Suppliers.Application.CommandServices;
using marketgo_backend.Suppliers.Application.QueryServices;
using marketgo_backend.Suppliers.Domain.Model.Commands;
using marketgo_backend.Suppliers.Domain.Model.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

var ownOrg = Guid.NewGuid();
var otherOrg = Guid.NewGuid();
var owner = new User("Ana", "ana@example.com", "hash", "Tienda A", "minimarket");
typeof(User).GetProperty(nameof(User.Id))!.SetValue(owner, new UserId(1));
typeof(User).GetProperty(nameof(User.OrganizationId))!.SetValue(owner, ownOrg);
var otherProfile = new Profile(otherOrg, "other@example.com", "supplier",
    "Proveedor B", "secret@example.com");
typeof(Profile).GetProperty(nameof(Profile.Id))!.SetValue(otherProfile, new ProfileId(2));
otherProfile.Update(new UpdateProfileCommand(2, otherOrg, null, null,
    "999000111", "Private street 1", "Miraflores", null, "Lima"));
var profiles = new ProfilesController(new NoProfileCommands(), new ProfileQueries(otherProfile), null!);
SetCaller(profiles, owner, "/api/v1/profiles");
var publicList = ((OkObjectResult)await profiles.GetAllProfiles(default)).Value!;
var publicProfile = ((IEnumerable<PublicProfileResource>)publicList).Single();
Check(publicProfile.OrganizationId == otherOrg && publicProfile.BusinessName == "Proveedor B", "Public profile identity");
Check(!publicProfile.GetType().GetProperties().Any(p => p.Name is "Email" or "Phone" or "Address" or "UserId"),
    "Profile list does not expose another organization's private fields");
var byId = ((OkObjectResult)await profiles.GetProfileById(2, default)).Value!;
Check(byId is PublicProfileResource, "Cross-organization profile detail stays public");

var otherProduct = new Product(otherOrg, "Yogurt", "Dairy", "Natural",
    DateOnly.FromDateTime(DateTime.Today.AddDays(10)), 14, 8m, null);
var products = new ProductsController(new NoProductCommands(), new ProductQueries(otherProduct));
SetCaller(products, owner, "/api/v1/products");
var catalog = ((IEnumerable<CatalogProductResource>)((OkObjectResult)await products.List(default)).Value!).Single();
Check(catalog.OrganizationId == otherOrg && catalog.Price == 8m, "Commercial catalog is visible");
Check(!catalog.GetType().GetProperties().Any(p => p.Name is "Quantity" or "ExpirationDate" or "SupplierId"),
    "Catalog omits another organization's private inventory fields");
Check(((OkObjectResult)await products.Get(otherProduct.Id, default)).Value is CatalogProductResource,
    "Cross-organization product detail stays commercial");
Check(await products.SupplierProducts(otherOrg, default) is ForbidResult,
    "Cannot read another supplier organization's private product list");

var requests = new RequisitionsController(new NoRequestCommands(), new NoRequestQueries());
SetCaller(requests, owner, $"/api/v1/minimarkets/{otherOrg}/requisitions");
Check(await requests.List(otherOrg, default) is ForbidResult, "Other minimarket requisitions denied");
SetCaller(requests, owner, $"/api/v1/suppliers/{ownOrg}/requisitions");
Check(await requests.List(ownOrg, default) is ForbidResult, "Wrong account type denied on supplier route");
var supplierAccount = new User("Bob", "bob@example.com", "hash", "Proveedor B", "supplier");
typeof(User).GetProperty(nameof(User.OrganizationId))!.SetValue(supplierAccount, otherOrg);
var supplierDirectory = new SuppliersController(new NoSupplierCommands(), new NoSupplierQueries());
SetCaller(supplierDirectory, supplierAccount, "/api/v1/suppliers");
Check(await supplierDirectory.List(default) is ForbidResult,
    "Supplier account cannot read minimarket-owned directory");

var directory = new SupplierProfile(ownOrg, "Proveedor B", "20600011122", "ventas@example.com",
    "999111222", "Lima", "Dairy", "Lima", otherOrg);
Check(SupplierResourceFromEntityAssembler.ToResourceFromEntity(directory).SupplierOrganizationId == otherOrg &&
      directory.Id != otherOrg, "Directory ID and supplier organization ID are distinct");
var requestRepository = new MemoryRequests();
var supplierLink = new SupplierLink();
var requestCommands = new SupplyRequestCommandService(requestRepository, new MemoryWork(),
    TimeProvider.System, supplierLink);
var createRequest = new CreateSupplyRequestCommand(ownOrg, otherOrg, 1, "Ana", "Proveedor B",
    "Restock", [new SupplyRequestItem("Yogurt", 2)]);
Check((await requestCommands.Handle(createRequest, default)).IsFailure && requestRepository.Request is null,
    "Unlinked supplier organization cannot receive a requisition");
supplierLink.IsLinked = true;
Check((await requestCommands.Handle(createRequest, default)).IsSuccess &&
      requestRepository.Request?.SupplierId == otherOrg,
    "Linked supplier organization's ID is stored in the requisition");
Console.WriteLine("Access smoke passed: tenant isolation, public projections, role routes and supplier identity.");

static void Check(bool value, string message) { if (!value) throw new Exception(message); }
static void SetCaller(ControllerBase controller, User caller, string path)
{
    var context = new DefaultHttpContext();
    context.Items["User"] = caller;
    context.Request.Path = path;
    controller.ControllerContext = new ControllerContext { HttpContext = context };
}
sealed class ProfileQueries(Profile profile) : IProfileQueryService
{
    public Task<IReadOnlyList<Profile>> Handle(GetAllProfilesQuery q, CancellationToken ct) => Task.FromResult<IReadOnlyList<Profile>>([profile]);
    public Task<Profile?> Handle(GetProfileByEmailQuery q, CancellationToken ct) => Task.FromResult<Profile?>(profile);
    public Task<Profile?> Handle(GetProfileByIdQuery q, CancellationToken ct) => Task.FromResult<Profile?>(profile);
}
sealed class NoProfileCommands : IProfileCommandService
{
    public Task<Result<Profile>> Handle(CreateProfileCommand q, CancellationToken ct) => throw new NotSupportedException();
    public Task<Result<Profile>> Handle(UpdateProfileCommand q, CancellationToken ct) => throw new NotSupportedException();
}
sealed class ProductQueries(Product product) : IProductQueryService
{
    public Task<IReadOnlyList<Product>> Handle(GetProductsQuery q, CancellationToken ct) => Task.FromResult<IReadOnlyList<Product>>([product]);
    public Task<Product?> Handle(GetProductByIdQuery q, CancellationToken ct) => Task.FromResult<Product?>(product);
}
sealed class NoProductCommands : IProductCommandService
{
    public Task<Result<Product>> Handle(CreateProductCommand q, CancellationToken ct) => throw new NotSupportedException();
    public Task<Result<Product>> Handle(UpdateProductCommand q, CancellationToken ct) => throw new NotSupportedException();
    public Task<Result<Product>> Withdraw(Guid id, Guid org, CancellationToken ct) => throw new NotSupportedException();
}
sealed class NoRequestQueries : ISupplyRequestQueryService
{
    public Task<IReadOnlyList<SupplyRequest>> Handle(GetSupplyRequestsQuery q, CancellationToken ct) =>
        throw new Exception("Forbidden request reached data access");
    public Task<SupplyRequest?> FindVisibleAsync(Guid id, Guid org, string type, CancellationToken ct) =>
        throw new Exception("Forbidden request reached data access");
}
sealed class NoRequestCommands : ISupplyRequestCommandService
{
    public Task<Result<SupplyRequest>> Handle(CreateSupplyRequestCommand q, CancellationToken ct) => throw new NotSupportedException();
    public Task<Result<SupplyRequest>> Handle(ReviewSupplyRequestCommand q, CancellationToken ct) => throw new NotSupportedException();
}

sealed class MemoryRequests : ISupplyRequestRepository
{
    public SupplyRequest? Request { get; private set; }
    public Task AddAsync(SupplyRequest request, CancellationToken ct) { Request = request; return Task.CompletedTask; }
    public Task<SupplyRequest?> FindAsync(Guid id, CancellationToken ct) => Task.FromResult(Request);
    public Task<IReadOnlyList<SupplyRequest>> ListForAsync(Guid org, string type, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<SupplyRequest>>([]);
}
sealed class MemoryWork : IUnitOfWork
{
    public Task CompleteAsync(CancellationToken ct = default) => Task.CompletedTask;
}
sealed class SupplierLink : ISuppliersContextFacade
{
    public bool IsLinked { get; set; }
    public Task<bool> IsLinkedActiveAsync(Guid minimarketId, Guid supplierOrganizationId, CancellationToken ct) =>
        Task.FromResult(IsLinked);
    public Task<bool> ExistsActiveAsync(Guid supplierId, CancellationToken ct) => Task.FromResult(false);
    public Task<int> CountActiveAsync(Guid organizationId, CancellationToken ct) => Task.FromResult(0);
}

sealed class NoSupplierQueries : ISupplierQueryService
{
    public Task<IReadOnlyList<SupplierProfile>> Handle(GetSuppliersQuery q, CancellationToken ct) =>
        throw new Exception("Forbidden request reached data access");
    public Task<SupplierProfile?> Handle(GetSupplierByIdQuery q, CancellationToken ct) =>
        throw new Exception("Forbidden request reached data access");
}
sealed class NoSupplierCommands : ISupplierCommandService
{
    public Task<Result<SupplierProfile>> Handle(CreateSupplierCommand q, CancellationToken ct) => throw new NotSupportedException();
    public Task<Result<SupplierProfile>> Handle(UpdateSupplierCommand q, CancellationToken ct) => throw new NotSupportedException();
}
