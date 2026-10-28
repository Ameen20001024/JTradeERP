namespace JTradeERP.Application.Features.MasterData.Customers.GetCustomers;
public sealed record GetCustomersResponse(Guid Id,string CustomerCode,string Name,string? BillingAddress,string? ShippingAddress,string? Phone,string? Email,string? TRN,decimal? CreditLimit);
