using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfers;
public sealed record GetStockTransfersQuery(Guid? SourceWarehouseId = null, Guid? DestinationWarehouseId = null,
    string? Status = null, int Page = 1, int PageSize = 50) : IRequest<GetStockTransfersResponse>;
