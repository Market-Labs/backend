using marketgo_backend.Products.Domain.Model.Events;
using marketgo_backend.Products.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Domain.Model.Aggregates.Internal;

namespace marketgo_backend.Products.Domain.Model.Aggregates;

public partial class Product : AggregateRoot
{
    private Product() { }
    public Product(Guid organizationId, string name, string category, string description,
        DateOnly expirationDate, int quantity, decimal price, Guid? supplierId)
    {
        if (organizationId == Guid.Empty) throw new ArgumentException("Organization is required.");
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        Update(name, category, description, expirationDate, quantity, price, true, supplierId);
        AddDomainEvent(new ProductCreatedEvent(Id, OrganizationId));
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

    public void Update(string name, string category, string description, DateOnly expirationDate,
        int quantity, decimal price, bool available, Guid? supplierId)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 160 ||
            string.IsNullOrWhiteSpace(category) || category.Trim().Length > 80 ||
            string.IsNullOrWhiteSpace(description) || description.Trim().Length > 1000 ||
            quantity < 0 || supplierId == Guid.Empty)
            throw new ArgumentException("Invalid product information.");
        var validatedPrice = new UnitPrice(price);
        Name = name.Trim();
        Category = category.Trim();
        Description = description.Trim();
        ExpirationDate = expirationDate;
        Quantity = quantity;
        Price = validatedPrice.Value;
        Available = available;
        SupplierId = supplierId;
    }
    public void Withdraw() => Available = false;
}
