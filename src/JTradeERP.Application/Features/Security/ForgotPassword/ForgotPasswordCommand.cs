using MediatR;
namespace JTradeERP.Application.Features.Security.ForgotPassword;
public sealed record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResponse>;
