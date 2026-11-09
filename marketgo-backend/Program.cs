using marketgo_backend.Iam.Application.Internal.CommandServices;
using marketgo_backend.Iam.Application.Services;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Iam.Infrastructure.Persistence.EFC.Repositories;
using marketgo_backend.Iam.Infrastructure.Security;
using marketgo_backend.Profiles.Domain.Repositories;
using marketgo_backend.Profiles.Infrastructure.Persistence.EFC.Repositories;
using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Repositories;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
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
    builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
    builder.Services.AddSingleton(TimeProvider.System);
    var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
    jwt.Validate();
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
                RoleClaimType = "role"
            };
            options.Events = new JwtBearerEvents { OnTokenValidated = JwtSessionValidation.ValidateAsync };
        });
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
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "MarketGo API v1"));
}

app.MapGet("/api/v1/health", () => Results.Ok(new { status = "Healthy", service = "MarketGo API" }))
    .WithName("GetHealth")
    .WithTags("Health");
if (databaseConfigured) app.MapControllers();

app.Run();
