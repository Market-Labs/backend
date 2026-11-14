using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using marketgo_backend.Iam.Domain.Model.Events;
using marketgo_backend.Iam.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Domain.Model.Aggregates.Internal;

namespace marketgo_backend.Iam.Domain.Model.Aggregates;

public partial class User : AggregateRoot
{
    private User() { }

    public User(string fullName, string email, string passwordHash, string organizationName, string accountType)
    {
        if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length > 160)
            throw new ArgumentException("Invalid full name.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(email) || email.Length > 254 || !new EmailAddressAttribute().IsValid(email))
            throw new ArgumentException("Invalid email.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(organizationName) || organizationName.Trim().Length > 160)
            throw new ArgumentException("Invalid organization name.", nameof(organizationName));
        if (accountType is not ("minimarket" or "supplier"))
            throw new ArgumentException("Account type must be minimarket or supplier.", nameof(accountType));

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        NormalizedEmail = Email.ToUpperInvariant();
        PasswordHash = passwordHash;
        OrganizationId = Guid.NewGuid();
        OrganizationName = organizationName.Trim();
        AccountType = accountType;
        Role = "owner";
        IsActive = true;
        AddDomainEvent(new UserRegisteredEvent(OrganizationId, Email, OrganizationName, AccountType));
    }

    public UserId Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    [JsonIgnore] public string PasswordHash { get; private set; } = string.Empty;
    public string OrganizationName { get; private set; } = string.Empty;
    public string AccountType { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset? LastSignedInAt { get; private set; }

    public User UpdateFullName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 160) throw new ArgumentException("Invalid full name.");
        FullName = name.Trim();
        return this;
    }

    public void RecordSignIn(DateTimeOffset at) => LastSignedInAt = at;
}
