using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Products.Domain.Model.Aggregates;

public sealed class Product : IAuditableEntity
{
    private Product() { }

    public Product(Guid organizationId, string name, string category, string description,
        DateOnly expirationDate, int quantity, decimal price, Guid? supplierId)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.", nameof(organizationId));
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 160) throw new ArgumentException("Invalid product name.", nameof(name));
        if (string.IsNullOrWhiteSpace(category) || category.Trim().Length > 80) throw new ArgumentException("Invalid category.", nameof(category));
        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length > 1000) throw new ArgumentException("Invalid description.", nameof(description));
        if (quantity < 0 || price <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be nonnegative and price positive.");
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        Name = name.Trim();
        Category = category.Trim();
        Description = description.Trim();
        ExpirationDate = expirationDate;
        Quantity = quantity;
        Price = price;
        SupplierId = supplierId;
        Available = true;
    }

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? SupplierId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateOnly ExpirationDate { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public bool Available { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void Update(string description, decimal price, int quantity, bool available)
    {
        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length > 1000 || price <= 0 || quantity < 0)
            throw new ArgumentException("Invalid product update.");
        Description = description.Trim();
        Price = price;
        Quantity = quantity;
        Available = available;
    }
}
