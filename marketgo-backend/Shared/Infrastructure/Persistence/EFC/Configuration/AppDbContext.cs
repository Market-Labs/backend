using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Domain.Model.Aggregates;
using marketgo_backend.Products.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Suppliers.Domain.Model.Aggregates;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>EF Core context shared by the bounded contexts.</summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<SupplierProfile> SupplierProfiles => Set<SupplierProfile>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<WasteRecord> WasteRecords => Set<WasteRecord>();
    public DbSet<DonationRecord> DonationRecords => Set<DonationRecord>();
    public DbSet<ConservationReading> ConservationReadings => Set<ConservationReading>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<SupplyRequest> SupplyRequests => Set<SupplyRequest>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.UseSnakeCaseNamingConvention();
    }
}
