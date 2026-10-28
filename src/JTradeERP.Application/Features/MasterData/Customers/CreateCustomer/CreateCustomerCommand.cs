using MediatR;
namespace JTradeERP.Application.Features.MasterData.Customers.CreateCustomer;
public sealed record CreateCustomerCommand(string CustomerCode,string Name,string? BillingAddress,string? ShippingAddress,string? Phone,string? Email,string? TRN,decimal? CreditLimit):IRequest<CreateCustomerResponse>;
