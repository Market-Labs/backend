using marketgo_backend.Shared.Domain.Model.Entities;
namespace marketgo_backend.Requisition.Domain.Model.Aggregates;
public partial class SupplyRequest : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
