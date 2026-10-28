namespace JTradeERP.Application.Features.MasterData.Customers.UpdateCustomer;
public sealed record UpdateCustomerResponse(Guid Id,string CustomerCode,string Name,string? BillingAddress,string? ShippingAddress,string? Phone,string? Email,string? TRN,decimal? CreditLimit);
