namespace marketgo_backend.Iam.Domain.Model.Aggregates;

public sealed class Session
{
    private Session() { } // EF Core

    public Session(Guid userId, string tokenHash, DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        if (userId == Guid.Empty) throw new ArgumentException("User is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(tokenHash) || tokenHash.Length != 64)
            throw new ArgumentException("A SHA-256 token hash is required.", nameof(tokenHash));
        if (expiresAt <= createdAt) throw new ArgumentException("Expiration must follow creation.", nameof(expiresAt));

        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public bool IsActiveAt(DateTimeOffset at) => RevokedAt is null && ExpiresAt > at;
    public void Revoke(DateTimeOffset at) => RevokedAt ??= at;
}
