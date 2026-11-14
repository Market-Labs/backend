using marketgo_backend.Dashboard.Application.Internal.QueryServices;
using marketgo_backend.Dashboard.Application.QueryServices;
namespace marketgo_backend.Dashboard.Infrastructure.Configuration;
public static class DashboardModuleExtensions
{
    public static IServiceCollection AddDashboardModule(this IServiceCollection services)
    {
        services.AddScoped<IDashboardQueryService, DashboardQueryService>();
        return services;
    }
}
