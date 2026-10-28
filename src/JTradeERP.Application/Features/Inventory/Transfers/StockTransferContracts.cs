namespace JTradeERP.Application.Features.Inventory.Transfers;

public sealed record StockTransferLineRequest(Guid ProductId, decimal Quantity);
public sealed record StockTransferLineResponse(Guid Id, Guid ProductId, string ProductCode, string ProductName, decimal Quantity);
public sealed record StockTransferResponse(Guid Id, Guid SourceWarehouseId, string SourceWarehouseCode,
    Guid DestinationWarehouseId, string DestinationWarehouseCode, string Status, DateTimeOffset? PostedAt,
    IReadOnlyCollection<StockTransferLineResponse> Lines);
