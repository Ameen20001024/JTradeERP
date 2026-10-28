using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Inventory;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.OpeningBalances.ReverseOpeningBalance;
public sealed class ReverseOpeningBalanceHandler(IApplicationDbContext db, IStockPostingService stock,
    ICurrentUserService actor, IClock clock) : IRequestHandler<ReverseOpeningBalanceCommand, ReverseOpeningBalanceResponse>
{
    public async Task<ReverseOpeningBalanceResponse> Handle(ReverseOpeningBalanceCommand request, CancellationToken cancellationToken)
    {
        var document = db.OpeningBalances.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Opening balance was not found.");
        var original = db.StockLedgerEntries.SingleOrDefault(x => x.SourceDocumentType == "OpeningBalance"
            && x.SourceDocumentId == document.SourceDocumentId && x.MovementType == StockMovementType.OpeningBalance)
            ?? throw new ConflictException("The opening-balance ledger entry was not found.");
        var prior = db.StockLedgerEntries.SingleOrDefault(x => x.ReversalOfLedgerEntryId == original.Id);
        if (prior is not null)
            return new(document.Id, original.Id, prior.Id, document.Status.ToString());
        if (document.Status != InventoryDocumentStatus.Posted)
            throw new ConflictException("Only posted opening balances can be reversed.");
        var actorId = actor.UserId ?? throw new AuthenticationFailureException();
        var reversal = stock.Reverse(original, document.Id, "OpeningBalanceReversal", actorId, InventoryPostingMetadata.CorrelationId);
        document.MarkReversed(clock.UtcNow, actorId);
        SecurityAuditWriter.Add(db, actorId, "OpeningBalanceReversed", "OpeningBalance", document.Id.ToString(),
            "Posted", "Reversed");
        await db.SaveChangesAsync(cancellationToken);
        return new(document.Id, original.Id, reversal.Id, document.Status.ToString());
    }
}
