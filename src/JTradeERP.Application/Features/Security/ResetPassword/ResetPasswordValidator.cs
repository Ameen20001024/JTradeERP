using FluentValidation;
namespace JTradeERP.Application.Features.Security.ResetPassword;
public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand> { public ResetPasswordValidator(){RuleFor(x=>x.Email).NotEmpty().EmailAddress();RuleFor(x=>x.Otp).NotEmpty().Length(6);RuleFor(x=>x.NewPassword).NotEmpty();} }
