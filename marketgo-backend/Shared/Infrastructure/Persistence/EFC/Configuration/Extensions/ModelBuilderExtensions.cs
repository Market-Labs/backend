using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;

/// <summary>Applies snake_case names to EF Core relational objects.</summary>
public static class ModelBuilderExtensions
{
    public static void UseSnakeCaseNamingConvention(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrWhiteSpace(tableName)) entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (!string.IsNullOrWhiteSpace(columnName)) property.SetColumnName(ToSnakeCase(columnName));
            }

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrWhiteSpace(keyName)) key.SetName(ToSnakeCase(keyName));
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var constraintName = foreignKey.GetConstraintName();
                if (!string.IsNullOrWhiteSpace(constraintName)) foreignKey.SetConstraintName(ToSnakeCase(constraintName));
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrWhiteSpace(indexName)) index.SetDatabaseName(ToSnakeCase(indexName));
            }
        }
    }

    private static string ToSnakeCase(string value)
    {
        var separatedAcronyms = Regex.Replace(value, "([A-Z]+)([A-Z][a-z])", "$1_$2");
        return Regex.Replace(separatedAcronyms, "([a-z0-9])([A-Z])", "$1_$2")
            .Replace('-', '_')
            .ToLowerInvariant();
    }
}
