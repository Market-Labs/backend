using marketgo_backend.Shared.Domain.Model.Entities;
namespace marketgo_backend.Communication.Domain.Model.Aggregates;
public partial class Alert : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
