using MediatR;
namespace JTradeERP.Application.Features.MasterData.Suppliers.GetSuppliers;
public sealed record GetSuppliersQuery:IRequest<IReadOnlyList<GetSuppliersResponse>>;
