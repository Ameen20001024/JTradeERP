using MediatR;
namespace JTradeERP.Application.Features.Security.ResetPassword;
public sealed record ResetPasswordCommand(string Email,string Otp,string NewPassword) : IRequest<ResetPasswordResponse>;
