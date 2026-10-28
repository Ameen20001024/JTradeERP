using MediatR;
namespace JTradeERP.Application.Features.MasterData.Customers.GetCustomerById;
public sealed record GetCustomerByIdQuery(Guid Id):IRequest<GetCustomerByIdResponse>;
