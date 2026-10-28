using MediatR;
namespace JTradeERP.Application.Features.MasterData.Suppliers.DeleteSupplier;
public sealed record DeleteSupplierCommand(Guid Id):IRequest;
