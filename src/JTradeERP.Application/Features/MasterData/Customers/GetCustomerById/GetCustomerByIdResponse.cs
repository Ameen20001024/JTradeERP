namespace JTradeERP.Application.Features.MasterData.Customers.GetCustomerById;
public sealed record GetCustomerByIdResponse(Guid Id,string CustomerCode,string Name,string? BillingAddress,string? ShippingAddress,string? Phone,string? Email,string? TRN,decimal? CreditLimit);
