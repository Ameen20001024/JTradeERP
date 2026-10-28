namespace JTradeERP.Application.Features.Inventory.OpeningBalances.ReverseOpeningBalance;
public sealed record ReverseOpeningBalanceResponse(Guid OpeningBalanceId, Guid OriginalLedgerEntryId,
    Guid ReversalLedgerEntryId, string Status);
