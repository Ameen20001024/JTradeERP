using MediatR;
namespace JTradeERP.Application.Features.Security.RefreshAccessToken;
public sealed record RefreshAccessTokenCommand(string? RefreshToken) : IRequest<RefreshAccessTokenResponse>;
