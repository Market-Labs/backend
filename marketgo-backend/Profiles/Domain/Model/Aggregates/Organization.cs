using marketgo_backend.Profiles.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Profiles.Domain.Model.Aggregates;

public sealed class Organization : IAuditableEntity
{
    private Organization() { } // EF Core

    public Organization(string name, OrganizationKind kind)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 160)
            throw new ArgumentException("Organization name must contain 1 to 160 characters.", nameof(name));
        if (!Enum.IsDefined(kind))
            throw new ArgumentOutOfRangeException(nameof(kind));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Kind = kind;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public OrganizationKind Kind { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
