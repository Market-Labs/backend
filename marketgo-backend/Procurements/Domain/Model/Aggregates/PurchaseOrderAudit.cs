using marketgo_backend.Shared.Domain.Model.Entities;
namespace marketgo_backend.Procurements.Domain.Model.Aggregates;
public partial class PurchaseOrder : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
