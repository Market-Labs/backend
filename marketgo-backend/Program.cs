using marketgo_backend.Iam.Application.Internal.CommandServices;
using marketgo_backend.Iam.Application.Services;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Iam.Infrastructure.Persistence.EFC.Repositories;
using marketgo_backend.Iam.Infrastructure.Security;
using marketgo_backend.Organizations.Domain.Repositories;
using marketgo_backend.Organizations.Infrastructure.Persistence.EFC.Repositories;
using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
{
    if (allowedOrigins.Length > 0)
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
}));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseConfigured = !string.IsNullOrWhiteSpace(connectionString);
if (databaseConfigured)
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString!));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
    builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ISessionRepository, SessionRepository>();
    builder.Services.AddScoped<marketgo_backend.Iam.Application.Services.IAuthenticationService, marketgo_backend.Iam.Application.Internal.CommandServices.AuthenticationService>();
    builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
    builder.Services.AddSingleton<ISessionTokenService, SessionTokenService>();
    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddAuthentication(SessionAuthenticationHandler.SchemeName)
        .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(
            SessionAuthenticationHandler.SchemeName, _ => { });
}

var app = builder.Build();

if (!databaseConfigured)
    app.Logger.LogWarning("MySQL is not configured. Set ConnectionStrings__DefaultConnection to enable persistence and IAM.");

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("Frontend");
if (databaseConfigured) app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/api/v1/health", () => Results.Ok(new { status = "Healthy", service = "MarketGo API" }))
    .WithName("GetHealth")
    .WithTags("Health");
if (databaseConfigured) app.MapControllers();

app.Run();
