using JTradeERP.Domain.Security;

namespace JTradeERP.Application.UnitTests;

public sealed class SecurityDomainTests
{
    [Fact]
    public void PasswordResetOtp_ExpiresAndLocksAfterConfiguredFailedAttempts()
    {
        var created = DateTimeOffset.UtcNow;
        var otp = new PasswordResetOtp(Guid.NewGuid(), "salted-hash", created, created.AddMinutes(10));

        Assert.True(otp.IsUsable(created.AddMinutes(1), maxAttempts: 3));
        otp.RecordFailure(maxAttempts: 3, created.AddMinutes(2));
        Assert.True(otp.IsUsable(created.AddMinutes(2), maxAttempts: 3));
        otp.RecordFailure(maxAttempts: 3, created.AddMinutes(3));
        Assert.True(otp.IsUsable(created.AddMinutes(3), maxAttempts: 3));
        otp.RecordFailure(maxAttempts: 3, created.AddMinutes(4));

        Assert.False(otp.IsUsable(created.AddMinutes(4), maxAttempts: 3));
        Assert.False(otp.IsUsable(created.AddMinutes(11), maxAttempts: 3));
    }

    [Fact]
    public void PasswordResetOtp_IsOneTimeAndCanBeInvalidated()
    {
        var now = DateTimeOffset.UtcNow;
        var used = new PasswordResetOtp(Guid.NewGuid(), "hash", now, now.AddMinutes(10));
        used.Use(now.AddMinutes(1));

        Assert.False(used.IsUsable(now.AddMinutes(1), maxAttempts: 5));

        var invalidated = new PasswordResetOtp(Guid.NewGuid(), "hash", now, now.AddMinutes(10));
        invalidated.Invalidate(now.AddMinutes(1));
        invalidated.Invalidate(now.AddMinutes(2));

        Assert.False(invalidated.IsUsable(now.AddMinutes(2), maxAttempts: 5));
        Assert.Equal(now.AddMinutes(1), invalidated.InvalidatedAt);
    }

    [Fact]
    public void RefreshToken_ActiveStateDependsOnExpiryAndRevocation()
    {
        var now = DateTimeOffset.UtcNow;
        var token = new RefreshToken(Guid.NewGuid(), "sha256-hash", now, now.AddDays(1));

        Assert.True(token.IsActive(now));
        Assert.False(token.IsActive(now.AddDays(1)));

        token.Revoke(now.AddMinutes(1), "logout");
        Assert.False(token.IsActive(now.AddMinutes(1)));
        Assert.Equal("logout", token.RevocationReason);
    }

    [Fact]
    public void PasswordAndStatusChangesRotateSecurityStamp()
    {
        var user = new User("operator", "operator@example.test", "password-hash");
        var initial = user.SecurityStamp;

        user.SetStatus(false);
        var deactivated = user.SecurityStamp;
        user.SetPasswordHash("new-password-hash");

        Assert.NotEqual(initial, deactivated);
        Assert.NotEqual(deactivated, user.SecurityStamp);
        Assert.False(user.IsActive);
    }
}
