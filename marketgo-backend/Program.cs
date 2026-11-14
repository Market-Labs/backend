using marketgo_backend.Suppliers.Application.Acl;
using marketgo_backend.Suppliers.Interfaces.Acl;
using marketgo_backend.Iam.Application.Acl;
using marketgo_backend.Iam.Application.CommandServices;
using marketgo_backend.Iam.Application.Internal.CommandServices;
using marketgo_backend.Iam.Application.Internal.OutboundServices;
using marketgo_backend.Iam.Application.Internal.QueryServices;
using marketgo_backend.Iam.Application.QueryServices;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Iam.Infrastructure.Hashing.BCrypt.Services;
using marketgo_backend.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using marketgo_backend.Iam.Infrastructure.Tokens.Jwt.Configuration;
using marketgo_backend.Iam.Infrastructure.Tokens.Jwt.Services;
using marketgo_backend.Iam.Interfaces.Acl;
using marketgo_backend.Profiles.Application.Acl;
using marketgo_backend.Profiles.Application.CommandServices;
using marketgo_backend.Profiles.Application.Internal.CommandServices;
using marketgo_backend.Profiles.Application.Internal.QueryServices;
using marketgo_backend.Profiles.Application.QueryServices;
using marketgo_backend.Profiles.Domain.Repositories;
using marketgo_backend.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using marketgo_backend.Profiles.Interfaces.Acl;
using marketgo_backend.Suppliers.Application.CommandServices;
using marketgo_backend.Suppliers.Application.Internal.CommandServices;
using marketgo_backend.Suppliers.Application.Internal.QueryServices;
using marketgo_backend.Suppliers.Application.QueryServices;
using marketgo_backend.Suppliers.Domain.Repositories;
using marketgo_backend.Suppliers.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using marketgo_backend.Shared.Infrastructure.Interfaces.AspNetCore.OpenApi;
using marketgo_backend.Shared.Infrastructure.Mediator.Cortex.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using marketgo_backend.Shared.Infrastructure.Pipeline.Middleware.Components;
using marketgo_backend.Shared.Interfaces.Rest.ProblemDetails;
using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddLocalization();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddAuthorization();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
builder.Services.AddSingleton<ProblemDetailsFactory>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
{
    if (allowedOrigins.Length > 0)
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
}));

builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(ExceptionHandlingCommandBehavior<>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(TransactionCommandBehavior<>));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseConfigured = !string.IsNullOrWhiteSpace(connectionString);
if (databaseConfigured)
{
    (builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>() ?? new TokenSettings()).Validate();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserCommandService, UserCommandService>();
    builder.Services.AddScoped<IUserQueryService, UserQueryService>();
    builder.Services.AddScoped<IHashingService, HashingService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();
    builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
    builder.Services.AddScoped<IProfileCommandService, ProfileCommandService>();
    builder.Services.AddScoped<IProfileQueryService, ProfileQueryService>();
    builder.Services.AddScoped<IProfilesContextFacade, ProfilesContextFacade>();
    builder.Services.AddCortexMediator([typeof(Program)]);
    builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
    builder.Services.AddScoped<ISupplierCommandService, SupplierCommandService>();
    builder.Services.AddScoped<ISupplierQueryService, SupplierQueryService>();
    builder.Services.AddScoped<ISuppliersContextFacade, SuppliersContextFacade>();
    builder.Services.AddScoped<DispatchDomainEventsInterceptor>();
    builder.Services.AddDbContext<AppDbContext>((services, options) =>
        options.UseMySQL(connectionString!).AddInterceptors(services.GetRequiredService<DispatchDomainEventsInterceptor>()));
}

var app = builder.Build();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseRouting();
app.Use(async (context, next) =>
{
    if (!databaseConfigured && context.Request.Path.StartsWithSegments("/api/v1") &&
        !context.Request.Path.StartsWithSegments("/api/v1/health"))
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsJsonAsync(new { error = "MySQL is not configured." });
        return;
    }
    await next(context);
});
if (databaseConfigured) app.UseRequestAuthorization();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "MarketGo API v1"));
}
app.MapGet("/api/v1/health", () => Results.Ok(new { status = "Healthy", service = "MarketGo API" }));
app.MapControllers();
app.Run();
