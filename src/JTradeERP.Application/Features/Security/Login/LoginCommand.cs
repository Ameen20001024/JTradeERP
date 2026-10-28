using MediatR;
namespace JTradeERP.Application.Features.Security.Login;
public sealed record LoginCommand(string UserName, string Password) : IRequest<LoginResponse>;
