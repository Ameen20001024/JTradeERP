using FluentValidation;
namespace JTradeERP.Application.Features.Security.ForgotPassword;
public sealed class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand> { public ForgotPasswordValidator(){RuleFor(x=>x.Email).NotEmpty().EmailAddress().MaximumLength(320);} }
