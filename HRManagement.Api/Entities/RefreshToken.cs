namespace HRManagement.Api.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = null!;

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public DateTime? RevokedAtUtc { get; private set; }

    public Guid? ReplacedByTokenId { get; private set; }

    public AppUser User { get; private set; } = null!;

    private RefreshToken()
    {
    }

    public RefreshToken(Guid userId, string tokenHash, DateTime expiresAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
    }

    public void Revoke(DateTime revokedAtUtc)
    {
        RevokedAtUtc ??= revokedAtUtc;
    }

    public void ReplaceWith(Guid replacementTokenId, DateTime revokedAtUtc)
    {
        Revoke(revokedAtUtc);
        ReplacedByTokenId = replacementTokenId;
    }
}