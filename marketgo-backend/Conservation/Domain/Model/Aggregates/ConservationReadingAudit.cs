using marketgo_backend.Shared.Domain.Model.Entities;
namespace marketgo_backend.Conservation.Domain.Model.Aggregates;
public partial class ConservationReading : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
