using MediatR;
namespace JTradeERP.Application.Features.MasterData.Customers.UpdateCustomer;
public sealed record UpdateCustomerCommand(Guid Id,string CustomerCode,string Name,string? BillingAddress,string? ShippingAddress,string? Phone,string? Email,string? TRN,decimal? CreditLimit):IRequest<UpdateCustomerResponse>;
