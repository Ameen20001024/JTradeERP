using MediatR;
namespace JTradeERP.Application.Features.Inventory.StockLedger.GetStockLedger;
public sealed record GetStockLedgerQuery(
    Guid? ProductId=null,
    Guid? WarehouseId=null,
    DateTimeOffset? From=null,
    DateTimeOffset? To=null,
    string? SourceDocumentType=null,
    int Page=1,
    int PageSize=100):IRequest<GetStockLedgerResponse>;
