using System.ComponentModel.DataAnnotations;
using marketgo_backend.Profiles.Domain.Model.Commands;
using marketgo_backend.Profiles.Domain.Model.Events;
using marketgo_backend.Profiles.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Domain.Model.Aggregates.Internal;

namespace marketgo_backend.Profiles.Domain.Model.Aggregates;

public partial class Profile : AggregateRoot
{
    private Profile() { }

    public Profile(Guid organizationId, string ownerEmail, string type, string businessName, string email)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        if (type is not ("minimarket" or "supplier")) throw new ArgumentException("Invalid account type.");
        OrganizationId = organizationId;
        OwnerEmail = ValidateEmail(ownerEmail);
        Type = type;
        BusinessName = Required(businessName, 160);
        Email = ValidateEmail(email);
        AddDomainEvent(new ProfileCreatedEvent(organizationId, Email));
    }

    public Profile(CreateProfileCommand command) : this(command.OrganizationId, command.OwnerEmail,
        command.Type, command.BusinessName, command.Email)
    {
        Update(new UpdateProfileCommand(0, command.OrganizationId, null, null, command.Phone,
            command.Address, command.District, command.Specialty, command.CoverageArea));
    }

    public ProfileId Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string OwnerEmail { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string BusinessName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public string Specialty { get; private set; } = string.Empty;
    public string CoverageArea { get; private set; } = string.Empty;

    public void Update(UpdateProfileCommand command)
    {
        if (command.BusinessName is not null) BusinessName = Required(command.BusinessName, 160);
        if (command.Email is not null) Email = ValidateEmail(command.Email);
        if (command.Phone is not null) Phone = Optional(command.Phone, 40);
        if (command.Address is not null) Address = Optional(command.Address, 250);
        if (command.District is not null) District = Optional(command.District, 120);
        if (command.Specialty is not null) Specialty = Optional(command.Specialty, 160);
        if (command.CoverageArea is not null) CoverageArea = Optional(command.CoverageArea, 160);
    }

    private static string Required(string value, int limit)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > limit)
            throw new ArgumentException("A required profile field is invalid.");
        return value.Trim();
    }
    private static string Optional(string value, int limit)
    {
        if (value.Trim().Length > limit) throw new ArgumentException("A profile field is too long.");
        return value.Trim();
    }
    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 254 || !new EmailAddressAttribute().IsValid(email))
            throw new ArgumentException("Invalid email.");
        return email.Trim().ToLowerInvariant();
    }
}
