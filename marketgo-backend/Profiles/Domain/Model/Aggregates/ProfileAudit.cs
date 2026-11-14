using marketgo_backend.Shared.Domain.Model.Entities;

namespace marketgo_backend.Profiles.Domain.Model.Aggregates;

public partial class Profile : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}