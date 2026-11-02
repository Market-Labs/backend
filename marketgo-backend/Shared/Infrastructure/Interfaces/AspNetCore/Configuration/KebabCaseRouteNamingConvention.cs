using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace marketgo_backend.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;

/// <summary>Replaces [controller] route tokens with kebab-case names.</summary>
public sealed class KebabCaseRouteNamingConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var routeName = ToKebabCase(controller.ControllerName);
        foreach (var selector in controller.Selectors.Concat(controller.Actions.SelectMany(action => action.Selectors)))
        {
            var route = selector.AttributeRouteModel;
            if (route?.Template is { } template)
                route.Template = template.Replace("[controller]", routeName, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static string ToKebabCase(string name)
    {
        var separatedAcronyms = Regex.Replace(name, "([A-Z]+)([A-Z][a-z])", "$1-$2");
        return Regex.Replace(separatedAcronyms, "([a-z0-9])([A-Z])", "$1-$2")
            .ToLowerInvariant();
    }
}
