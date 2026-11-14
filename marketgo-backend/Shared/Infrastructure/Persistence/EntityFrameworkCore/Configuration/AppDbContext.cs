using marketgo_backend.Procurements.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using marketgo_backend.Requisition.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using marketgo_backend.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using marketgo_backend.Products.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using marketgo_backend.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using marketgo_backend.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using marketgo_backend.Suppliers.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyIamConfiguration();
        builder.ApplyProfilesConfiguration();
        builder.ApplySuppliersConfiguration();
        builder.ApplyProductsConfiguration();
        builder.ApplyInventoryConfiguration();
        builder.ApplyRequisitionConfiguration();
        builder.ApplyProcurementsConfiguration();
        builder.UseSnakeCaseNamingConvention();
    }
}
