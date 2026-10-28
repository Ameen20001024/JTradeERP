namespace JTradeERP.Domain.Security;

public enum Capability
{
    None = 0,
    View = 1,
    CreateEdit = 2,
    Post = 3,
    Full = 4
}

public sealed class User
{
    private User() { }
    public User(string userName, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        UserName = userName.Trim();
        Email = email.Trim();
        PasswordHash = passwordHash;
        IsActive = true;
        SecurityStamp = Guid.NewGuid();
    }
    public Guid Id { get; private set; }
    public string UserName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public Guid SecurityStamp { get; private set; }
    public void SetStatus(bool isActive) { IsActive = isActive; SecurityStamp = Guid.NewGuid(); }
    public void SetPasswordHash(string passwordHash) { PasswordHash = passwordHash; SecurityStamp = Guid.NewGuid(); }
}

public sealed class Role
{
    private Role() { }
    public Role(string name) { Id = Guid.NewGuid(); Name = name.Trim(); }
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public void Rename(string name) => Name = name.Trim();
}

public sealed class Permission
{
    private Permission() { }
    public Permission(Guid id, string code) { Id = id; Code = code; }
    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
}

public sealed class UserRole
{
    private UserRole() { }
    public UserRole(Guid userId, Guid roleId) { UserId = userId; RoleId = roleId; }
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
}

public sealed class RolePermission
{
    private RolePermission() { }
    public RolePermission(Guid roleId,
        Guid permissionId, 
        Capability capability) 
    { 
        RoleId = roleId; 
        PermissionId = permissionId;
        Capability = capability; 
    }
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public Capability Capability { get; private set; }
    public void SetCapability(Capability capability) => Capability = capability;
}

public sealed class RefreshToken
{
    private RefreshToken() { }
    public RefreshToken(Guid userId,
        string tokenHash,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt,
        Guid? parentTokenId = null,
        Guid? familyId = null)
    {
        Id = Guid.NewGuid();
        UserId = userId; 
        TokenHash = tokenHash; 
        CreatedAt = createdAt;
        ExpiresAt = expiresAt; 
        ParentTokenId = parentTokenId; 
        FamilyId = familyId ?? Id; 
    }
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public Guid? ParentTokenId { get; private set; }
    public Guid FamilyId { get; private set; }
    public byte[] Version { get; private set; } = [];
    public string? RevocationReason { get; private set; }
    public bool IsActive(DateTimeOffset now) => RevokedAt is null && ExpiresAt > now;
    public void Revoke(DateTimeOffset when, string reason, Guid? replacementId = null) 
    {
        RevokedAt = when; RevocationReason = reason; ReplacedByTokenId = replacementId;
    }
}

public sealed class PasswordResetOtp
{
    private PasswordResetOtp() { }
    public PasswordResetOtp(Guid userId, string otpHash, DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        OtpHash = otpHash;
        CreatedAt = createdAt; 
        ExpiresAt = expiresAt;
    }
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string OtpHash { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? UsedAt { get; private set; }
    public int FailedAttemptCount { get; private set; }
    public DateTimeOffset? InvalidatedAt { get; private set; }
    public bool IsUsable(DateTimeOffset now, int maxAttempts) => 
        
        UsedAt is null && InvalidatedAt is null && ExpiresAt > now && FailedAttemptCount < maxAttempts;
    
    public void RecordFailure(int maxAttempts, DateTimeOffset now) 
    {
        FailedAttemptCount++; if (FailedAttemptCount >= maxAttempts) InvalidatedAt = now; 
    }
    public void Use(DateTimeOffset now) => UsedAt = now;
    public void Invalidate(DateTimeOffset now) => InvalidatedAt ??= now;
}

public sealed class SecurityAuditEntry
{
    private SecurityAuditEntry() { }
    public SecurityAuditEntry(Guid? actorUserId,
        string action,
        string entity, 
        string entityId, 
        string? oldValue, 
        string? newValue, 
        string correlationId)
    { 
        Id = Guid.NewGuid(); 
        ActorUserId = actorUserId; 
        Action = action; 
        Entity = entity; 
        EntityId = entityId;
        OldValue = oldValue;
        NewValue = newValue;
        CorrelationId = correlationId; 
        OccurredAt = DateTimeOffset.UtcNow; 
    }
    public Guid Id { get; private set; }
    public Guid? ActorUserId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string Action { get; private set; } = null!;
    public string Entity { get; private set; } = null!;
    public string EntityId { get; private set; } = null!;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public string CorrelationId { get; private set; } = null!;
}
