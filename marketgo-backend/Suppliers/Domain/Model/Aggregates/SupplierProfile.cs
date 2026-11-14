using System.ComponentModel.DataAnnotations;
using marketgo_backend.Shared.Domain.Model.Aggregates.Internal;
using marketgo_backend.Suppliers.Domain.Model.Events;
using marketgo_backend.Suppliers.Domain.Model.ValueObjects;

namespace marketgo_backend.Suppliers.Domain.Model.Aggregates;

public partial class SupplierProfile : AggregateRoot
{
    private SupplierProfile() { }

    public SupplierProfile(Guid organizationId, string businessName, string ruc, string email,
        string phone, string address, string specialty, string coverageArea)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        Update(businessName, ruc, email, phone, address, specialty, coverageArea, "active");
        AddDomainEvent(new SupplierRegisteredEvent(Id, OrganizationId));
    }

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string BusinessName { get; private set; } = string.Empty;
    public string Ruc { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string Specialty { get; private set; } = string.Empty;
    public string CoverageArea { get; private set; } = string.Empty;
    public string Status { get; private set; } = "active";

    public void Update(string businessName, string ruc, string email, string phone,
        string address, string specialty, string coverageArea, string status)
    {
        var validatedRuc = new RucNumber(ruc);
        if (string.IsNullOrWhiteSpace(businessName) || businessName.Trim().Length > 160 ||
            string.IsNullOrWhiteSpace(email) || email.Length > 254 || !new EmailAddressAttribute().IsValid(email) ||
            string.IsNullOrWhiteSpace(phone) || phone.Trim().Length > 40 ||
            string.IsNullOrWhiteSpace(address) || address.Trim().Length > 250 ||
            string.IsNullOrWhiteSpace(specialty) || specialty.Trim().Length > 100 ||
            string.IsNullOrWhiteSpace(coverageArea) || coverageArea.Trim().Length > 150 ||
            status is not ("active" or "inactive"))
            throw new ArgumentException("Invalid supplier information.");
        BusinessName = businessName.Trim();
        Ruc = validatedRuc.Value;
        Email = email.Trim().ToLowerInvariant();
        Phone = phone.Trim();
        Address = address.Trim();
        Specialty = specialty.Trim();
        CoverageArea = coverageArea.Trim();
        Status = status;
    }
}
