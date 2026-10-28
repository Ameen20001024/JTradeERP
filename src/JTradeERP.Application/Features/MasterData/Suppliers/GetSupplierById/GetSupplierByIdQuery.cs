using MediatR;
namespace JTradeERP.Application.Features.MasterData.Suppliers.GetSupplierById;
public sealed record GetSupplierByIdQuery(Guid Id):IRequest<GetSupplierByIdResponse>;
