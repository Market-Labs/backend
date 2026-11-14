using marketgo_backend.Shared.Domain.Model.Entities;
namespace marketgo_backend.Products.Domain.Model.Aggregates;
public partial class Product : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
