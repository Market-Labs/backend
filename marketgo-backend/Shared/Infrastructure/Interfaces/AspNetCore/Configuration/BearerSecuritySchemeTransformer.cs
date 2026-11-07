using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace marketgo_backend.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;

public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            }
        };
        foreach (var (path, item) in document.Paths)
        {
            if (path is "/api/v1/health" or "/api/v1/auth/sign-up" or "/api/v1/auth/sign-in") continue;
            if (item.Operations is null) continue;
            foreach (var operation in item.Operations.Values)
            {
                operation.Security ??= [];
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        }
        return Task.CompletedTask;
    }
}
