using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JTradeERP.Infrastructure.Security;

public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<PasswordService> hasher = new();
    public string Hash(string password) => hasher.HashPassword(this, password);
    public bool Verify(string hash, string password) => hasher.VerifyHashedPassword(this, hash, password) != PasswordVerificationResult.Failed;
}

public sealed class JwtTokenService(IOptions<JwtOptions> options, IClock clock) : ITokenService
{
    private readonly JwtOptions options = options.Value;
    public AccessTokenResult CreateAccessToken(User user, IReadOnlyCollection<string> roles)
    {
        var expires = clock.UtcNow.AddMinutes(options.AccessTokenMinutes);
        var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new(ClaimTypes.NameIdentifier, user.Id.ToString()), new(ClaimTypes.Name, user.UserName), new("sst", user.SecurityStamp.ToString()), new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(options.Issuer, options.Audience, claims, notBefore: clock.UtcNow.UtcDateTime, expires: expires.UtcDateTime, signingCredentials: credentials);
        return new(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
    public string CreateOpaqueToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public string HashOpaqueToken(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;
    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;
    public Guid? UserId => Guid.TryParse(Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
    public string? UserName => Principal?.Identity?.Name;
}

public sealed class RefreshTokenCookie(IHttpContextAccessor accessor, IOptions<SecurityOptions> options) : IRefreshTokenCookie
{
    private static CookieOptions Cookie(DateTimeOffset? expires = null) => new() { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Path = "/api/auth", Expires = expires };
    public string? Read() => accessor.HttpContext?.Request.Cookies[options.Value.RefreshCookieName];
    public void Write(string token, DateTimeOffset expiresAt) => accessor.HttpContext?.Response.Cookies.Append(options.Value.RefreshCookieName, token, Cookie(expiresAt));
    public void Clear() => accessor.HttpContext?.Response.Cookies.Delete(options.Value.RefreshCookieName, Cookie(DateTimeOffset.UnixEpoch));
}

public sealed class EmailOptions
{
    public const string SectionName = "Email";
    public string Host { get; init; } = null!;
    public int Port { get; init; } = 587;
    public string From { get; init; } = null!;
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public bool EnableSsl { get; init; } = true;
}

public sealed class SmtpEmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    public async Task SendPasswordResetOtpAsync(string email, string otp, CancellationToken cancellationToken)
    {
        var config = options.Value;
        if (string.IsNullOrWhiteSpace(config.Host) || string.IsNullOrWhiteSpace(config.From))
            throw new InvalidOperationException("Email:Host and Email:From must be configured to send password recovery email.");
        using var message = new System.Net.Mail.MailMessage(config.From, email, "JTradeERP password recovery", $"Your password recovery code is {otp}. It expires shortly. If you did not request a reset, ignore this email.");
        using var client = new System.Net.Mail.SmtpClient(config.Host, config.Port) { EnableSsl = config.EnableSsl };
        if (!string.IsNullOrWhiteSpace(config.UserName)) client.Credentials = new System.Net.NetworkCredential(config.UserName, config.Password);
        cancellationToken.ThrowIfCancellationRequested();
#pragma warning disable SYSLIB0014
        await client.SendMailAsync(message, cancellationToken);
#pragma warning restore SYSLIB0014
    }
}
