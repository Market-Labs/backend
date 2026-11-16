using System.ComponentModel.DataAnnotations;

namespace marketgo_backend.Profiles.Domain.Model.Aggregates;

public sealed class ClientContact
{
    private ClientContact() { }

    public ClientContact(Guid supplierOrganizationId, string businessName, string email, string phone, string address, string district)
    {
        if (supplierOrganizationId == Guid.Empty) throw new ArgumentException("Supplier organization is required.");
        Id = Guid.NewGuid();
        SupplierOrganizationId = supplierOrganizationId;
        Update(businessName, email, phone, address, district);
    }

    public Guid Id { get; private set; }
    public Guid SupplierOrganizationId { get; private set; }
    public string BusinessName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;

    public void Update(string businessName, string email, string phone, string address, string district)
    {
        if (string.IsNullOrWhiteSpace(businessName) || businessName.Trim().Length > 160 ||
            string.IsNullOrWhiteSpace(email) || email.Length > 254 || !new EmailAddressAttribute().IsValid(email) ||
            string.IsNullOrWhiteSpace(phone) || phone.Trim().Length > 40 ||
            string.IsNullOrWhiteSpace(address) || address.Trim().Length > 250 ||
            string.IsNullOrWhiteSpace(district) || district.Trim().Length > 120)
            throw new ArgumentException("Invalid client contact.");
        BusinessName = businessName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone.Trim();
        Address = address.Trim();
        District = district.Trim();
    }
}