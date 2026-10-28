using FluentValidation;
namespace JTradeERP.Application.Features.Security.Login;
public sealed class LoginValidator : AbstractValidator<LoginCommand> { public LoginValidator() { RuleFor(x => x.UserName).NotEmpty(); RuleFor(x => x.Password).NotEmpty(); } }
