using JTradeERP.Domain.Inventory;

namespace JTradeERP.Application.Common.Inventory;

/// <summary>
/// Transaction-compatible stock primitives. Callers add their source document and invoke SaveChanges once
/// so the source document, ledger, balance, and audit effects share the caller's database transaction.
/// </summary>
public interface IStockPostingService
{
    StockLedgerEntry Receive(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        string sourceDocumentType, Guid sourceDocumentId, Guid? sourceLineId,
        StockMovementType movementType, Guid actorUserId, string correlationId);

    StockLedgerEntry Issue(
        Guid productId, Guid warehouseId, decimal quantity,
        string sourceDocumentType, Guid sourceDocumentId, Guid? sourceLineId,
        StockMovementType movementType, Guid actorUserId, string correlationId);

    void Reserve(Guid productId, Guid warehouseId, decimal quantity);
    void ReleaseReservation(Guid productId, Guid warehouseId, decimal quantity);
    StockLedgerEntry ConvertReservationToIssue(Guid productId, Guid warehouseId, decimal quantity,
        string sourceDocumentType, Guid sourceDocumentId, Guid sourceLineId,
        Guid actorUserId, string correlationId);

    StockBalance GetOrCreateBalance(Guid productId, Guid warehouseId);
    void EnsureActiveReferences(Guid productId, Guid warehouseId);
    StockLedgerEntry Reverse(StockLedgerEntry original, Guid reversalDocumentId, string reversalDocumentType, Guid actorUserId, string correlationId);
}
