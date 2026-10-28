using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Inventory;
using MediatR;

namespace JTradeERP.Application.Features.Inventory.OpeningBalances.PostOpeningBalance;

public sealed class PostOpeningBalanceHandler(
    IApplicationDbContext db,
    IStockPostingService stock,
    ICurrentUserService currentUser,
    IClock clock) : IRequestHandler<PostOpeningBalanceCommand, PostOpeningBalanceResponse>
{
    public async Task<PostOpeningBalanceResponse> Handle(PostOpeningBalanceCommand request, CancellationToken cancellationToken)
    {
        var existing = db.OpeningBalances.SingleOrDefault(x => x.SourceDocumentId == request.SourceDocumentId);
        if (existing is not null)
        {
            if (existing.ProductId != request.ProductId || existing.WarehouseId != request.WarehouseId
                || existing.Quantity != request.Quantity || existing.UnitCost != request.UnitCost)
                throw new ConflictException("The opening-balance source reference was already posted with different values.");
            return Map(existing);
        }

        var actorId = currentUser.UserId ?? throw new AuthenticationFailureException();
        stock.EnsureActiveReferences(request.ProductId, request.WarehouseId);
        var now = clock.UtcNow;
        var document = new OpeningBalance(request.SourceDocumentId, request.ProductId, request.WarehouseId,
            request.Quantity, request.UnitCost, now, actorId);
        db.Add(document);
        stock.Receive(request.ProductId, request.WarehouseId, request.Quantity, request.UnitCost,
            "OpeningBalance", request.SourceDocumentId, null, StockMovementType.OpeningBalance,
            actorId, InventoryPostingMetadata.CorrelationId);
        SecurityAuditWriter.Add(db, actorId, "OpeningBalancePosted", "OpeningBalance", document.Id.ToString(),
            null, $"{request.ProductId}|{request.WarehouseId}|{request.Quantity}|{request.UnitCost}");
        await db.SaveChangesAsync(cancellationToken);
        return Map(document);
    }

    private static PostOpeningBalanceResponse Map(OpeningBalance document) => new(
        document.Id, document.SourceDocumentId, document.ProductId, document.WarehouseId,
        document.Quantity, document.UnitCost, document.Status.ToString(), document.PostedAt);
}
