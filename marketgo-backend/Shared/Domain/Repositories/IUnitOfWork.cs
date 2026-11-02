namespace marketgo_backend.Shared.Domain.Repositories;

/// <summary>Persists changes made by repositories in the current request scope.</summary>
public interface IUnitOfWork
{
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
