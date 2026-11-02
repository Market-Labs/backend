namespace marketgo_backend.Shared.Domain.Model;

/// <summary>Marks an entity whose UTC audit timestamps are managed by persistence.</summary>
public interface IAuditableEntity
{
    DateTimeOffset? CreatedAt { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
}
