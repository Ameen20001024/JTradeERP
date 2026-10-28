using MediatR;
namespace JTradeERP.Application.Features.MasterData.Customers.GetCustomers;
public sealed record GetCustomersQuery:IRequest<IReadOnlyList<GetCustomersResponse>>;
