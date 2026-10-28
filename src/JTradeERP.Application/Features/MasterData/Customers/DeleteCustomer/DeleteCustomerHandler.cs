using MediatR;using JTradeERP.Application.Common.Persistence;using JTradeERP.Application.Common.Exceptions;
namespace JTradeERP.Application.Features.MasterData.Customers.DeleteCustomer;
public sealed class DeleteCustomerHandler(IApplicationDbContext db):IRequestHandler<DeleteCustomerCommand>{public async Task Handle(DeleteCustomerCommand r,CancellationToken ct){var e=db.Customers.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Customer not found.");db.Remove(e);await db.SaveChangesAsync(ct);}}
