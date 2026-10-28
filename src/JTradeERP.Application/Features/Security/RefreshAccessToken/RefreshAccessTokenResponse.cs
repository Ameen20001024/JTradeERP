namespace JTradeERP.Application.Features.Security.RefreshAccessToken;
public sealed record RefreshAccessTokenResponse(string AccessToken, DateTimeOffset ExpiresAt);
