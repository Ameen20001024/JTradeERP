using MediatR;
namespace JTradeERP.Application.Features.Security.Logout;
public sealed record LogoutCommand(Guid? UserId,string? RefreshToken) : IRequest;
