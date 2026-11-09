using System.ComponentModel.DataAnnotations;
using marketgo_backend.Profiles.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Profiles.Domain.Model.Aggregates;

public sealed class Organization : IAuditableEntity
{
    private Organization() { }

    public Organization(string name, OrganizationKind kind)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 160)
            throw new ArgumentException("Organization name must contain 1 to 160 characters.", nameof(name));
        if (!Enum.IsDefined(kind)) throw new ArgumentOutOfRangeException(nameof(kind));
        Id = Guid.NewGuid();
        Name = name.Trim();
        Kind = kind;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public OrganizationKind Kind { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string Specialty { get; private set; } = string.Empty;
    public string CoverageArea { get; private set; } = string.Empty;
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void UpdateContact(string? name, string? email, string? phone, string? address,
        string? district, string? specialty, string? coverageArea)
    {
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 160) throw new ArgumentException("Invalid business name.");
            Name = name.Trim();
        }
        if (email is not null)
        {
            if (email.Length > 254 || (email.Length > 0 && !new EmailAddressAttribute().IsValid(email)))
                throw new ArgumentException("Invalid email.");
            Email = email.Trim().ToLowerInvariant();
        }
        if (phone is not null) Phone = ValidateOptional(phone, 40, "phone");
        if (address is not null) Address = ValidateOptional(address, 250, "address");
        if (district is not null) District = ValidateOptional(district, 100, "district");
        if (specialty is not null) Specialty = ValidateOptional(specialty, 100, "specialty");
        if (coverageArea is not null) CoverageArea = ValidateOptional(coverageArea, 150, "coverage area");
    }

    private static string ValidateOptional(string value, int maxLength, string field)
    {
        if (value.Trim().Length > maxLength) throw new ArgumentException($"Invalid {field}.");
        return value.Trim();
    }
}
