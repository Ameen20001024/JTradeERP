using MediatR;
namespace JTradeERP.Application.Features.Security.GetMe;
public sealed record GetMeQuery : IRequest<GetMeResponse>;
