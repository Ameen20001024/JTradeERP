using MediatR;

namespace JTradeERP.Application.Features.Inventory.OpeningBalances.PostOpeningBalance;

public sealed record PostOpeningBalanceCommand(
    Guid SourceDocumentId,
    Guid ProductId,
    Guid WarehouseId,
    decimal Quantity,
    decimal UnitCost) : IRequest<PostOpeningBalanceResponse>;
