using marketgo_backend.Iam.Application.Internal.OutboundServices;
using marketgo_backend.Iam.Application.QueryServices;
using marketgo_backend.Iam.Domain.Model.Queries;

namespace marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Components;

public sealed class RequestAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IServiceProvider services)
    {
        var header = context.Request.Headers.Authorization.ToString();
        if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var tokens = services.GetRequiredService<ITokenService>();
            var id = await tokens.ValidateToken(header[7..].Trim());
            if (id is > 0)
            {
                var users = services.GetRequiredService<IUserQueryService>();
                var user = await users.Handle(new GetUserByIdQuery(id.Value), context.RequestAborted);
                if (user is { IsActive: true }) context.Items["User"] = user;
            }
        }
        await next(context);
    }
}
