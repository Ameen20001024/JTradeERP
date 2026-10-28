using MediatR;
namespace JTradeERP.Application.Features.MasterData.Customers.DeleteCustomer;
public sealed record DeleteCustomerCommand(Guid Id):IRequest;
