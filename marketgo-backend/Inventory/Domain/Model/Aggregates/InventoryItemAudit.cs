using marketgo_backend.Shared.Domain.Model.Entities;
namespace marketgo_backend.Inventory.Domain.Model.Aggregates;
public partial class InventoryItem : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
