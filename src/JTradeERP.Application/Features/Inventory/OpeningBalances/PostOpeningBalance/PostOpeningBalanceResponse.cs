namespace JTradeERP.Application.Features.Inventory.OpeningBalances.PostOpeningBalance;

public sealed record PostOpeningBalanceResponse(
    Guid Id,
    Guid SourceDocumentId,
    Guid ProductId,
    Guid WarehouseId,
    decimal Quantity,
    decimal UnitCost,
    string Status,
    DateTimeOffset PostedAt);
