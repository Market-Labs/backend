using System.Text.Json.Serialization;
using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Iam.Domain.Model.Aggregates;

public sealed class User : IAuditableEntity
{
    private User() { } // EF Core

    public User(Guid organizationId, string fullName, string email, string passwordHash)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length > 160)
            throw new ArgumentException("Full name must contain 1 to 160 characters.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(email) || email.Trim().Length > 254)
            throw new ArgumentException("Email must contain 1 to 254 characters.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        NormalizedEmail = Email.ToUpperInvariant();
        PasswordHash = passwordHash;
        Role = "owner";
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    [JsonIgnore] public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset? LastSignedInAt { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void RecordSignIn(DateTimeOffset at) => LastSignedInAt = at;
}
