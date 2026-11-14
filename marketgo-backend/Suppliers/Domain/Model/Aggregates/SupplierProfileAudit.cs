using marketgo_backend.Shared.Domain.Model.Entities;
namespace marketgo_backend.Suppliers.Domain.Model.Aggregates;
public partial class SupplierProfile : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
