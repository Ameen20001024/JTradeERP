namespace JTradeERP.Application.Features.Security.Login;
public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAt, LoginUserResponse User);
public sealed record LoginUserResponse(Guid Id, string UserName, string Email, IReadOnlyCollection<string> Roles);
