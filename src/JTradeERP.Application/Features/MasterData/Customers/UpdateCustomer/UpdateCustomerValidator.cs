using FluentValidation;
namespace JTradeERP.Application.Features.MasterData.Customers.UpdateCustomer;
public sealed class UpdateCustomerValidator:AbstractValidator<UpdateCustomerCommand>{public UpdateCustomerValidator(){RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.CustomerCode).NotEmpty().MaximumLength(50);RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);RuleFor(x=>x.CreditLimit).GreaterThanOrEqualTo(0).When(x=>x.CreditLimit.HasValue);RuleFor(x=>x.Email).EmailAddress().When(x=>!string.IsNullOrWhiteSpace(x.Email));}}
