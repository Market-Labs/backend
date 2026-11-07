using System.ComponentModel.DataAnnotations;
using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Organizations.Domain.Model.Aggregates;

public sealed class SupplierProfile : IAuditableEntity
{
    private SupplierProfile() { }

    public SupplierProfile(Guid organizationId, string businessName, string ruc, string email, string phone,
        string address, string specialty, string coverageArea)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        Update(businessName, ruc, email, phone, address, specialty, coverageArea, "active");
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
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void Update(string businessName, string ruc, string email, string phone,
        string address, string specialty, string coverageArea, string status)
    {
        if (string.IsNullOrWhiteSpace(businessName) || businessName.Trim().Length > 160 ||
            ruc.Length != 11 || !ruc.All(char.IsDigit) ||
            !new EmailAddressAttribute().IsValid(email) || email.Length > 254 ||
            string.IsNullOrWhiteSpace(phone) || phone.Length > 40 ||
            string.IsNullOrWhiteSpace(address) || address.Length > 250 ||
            string.IsNullOrWhiteSpace(specialty) || specialty.Length > 100 ||
            string.IsNullOrWhiteSpace(coverageArea) || coverageArea.Length > 150 ||
            status is not ("active" or "inactive")) throw new ArgumentException("Invalid supplier profile.");
        BusinessName = businessName.Trim();
        Ruc = ruc;
        Email = email.Trim().ToLowerInvariant();
        Phone = phone.Trim();
        Address = address.Trim();
        Specialty = specialty.Trim();
        CoverageArea = coverageArea.Trim();
        Status = status;
    }
}
