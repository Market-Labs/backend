using marketgo_backend.Analytics.Application.Internal.QueryServices;
using marketgo_backend.Analytics.Application.QueryServices;
namespace marketgo_backend.Analytics.Infrastructure.Configuration;
public static class AnalyticsModuleExtensions
{
    public static IServiceCollection AddAnalyticsModule(this IServiceCollection services)
    {
        services.AddScoped<IAnalyticsQueryService, AnalyticsQueryService>();
        return services;
    }
}
