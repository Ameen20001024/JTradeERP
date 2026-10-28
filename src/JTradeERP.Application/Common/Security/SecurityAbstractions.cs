using JTradeERP.Domain.Security;

namespace JTradeERP.Application.Common.Security;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}
public interface IPasswordService { string Hash(string password); bool Verify(string hash, string password); }
public interface ITokenService { AccessTokenResult CreateAccessToken(User user, IReadOnlyCollection<string> roles); string CreateOpaqueToken(); string HashOpaqueToken(string value); }
public sealed record AccessTokenResult(string AccessToken, DateTimeOffset ExpiresAt);
public interface IEmailSender { Task SendPasswordResetOtpAsync(string email, string otp, CancellationToken cancellationToken); }
public interface IRefreshTokenCookie { string? Read(); void Write(string token, DateTimeOffset expiresAt); void Clear(); }
public interface IClock { DateTimeOffset UtcNow { get; } }
public sealed class SystemClock : IClock { public DateTimeOffset UtcNow => DateTimeOffset.UtcNow; }
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string SigningKey { get; init; } = null!;
    public int AccessTokenMinutes { get; init; } = 15;
}
public sealed class SecurityOptions
{
    public const string SectionName = "Security";
    public int RefreshTokenDays { get; init; } = 7;
    public int OtpMinutes { get; init; } = 10;
    public int OtpMaxAttempts { get; init; } = 5;
    public int OtpResendCooldownSeconds { get; init; } = 60;
    public string RefreshCookieName { get; init; } = "jtrade_refresh";
}
