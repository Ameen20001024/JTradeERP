using MediatR;using JTradeERP.Application.Common.Persistence;using JTradeERP.Application.Common.Exceptions;
namespace JTradeERP.Application.Features.MasterData.Suppliers.DeleteSupplier;
public sealed class DeleteSupplierHandler(IApplicationDbContext db):IRequestHandler<DeleteSupplierCommand>{public async Task Handle(DeleteSupplierCommand r,CancellationToken ct){var e=db.Suppliers.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Supplier not found.");db.Remove(e);await db.SaveChangesAsync(ct);}}
