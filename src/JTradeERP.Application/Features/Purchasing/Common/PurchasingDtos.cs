using JTradeERP.Domain.Purchasing;

namespace JTradeERP.Application.Features.Purchasing.Common;

public sealed record PurchaseOrderLineInput(Guid ProductId, decimal OrderedQuantity, decimal UnitPurchaseCost,
    Guid TaxCodeId, decimal DiscountAmount = 0m);

public sealed record GoodsReceiptNoteLineInput(Guid ProductId, Guid? PurchaseOrderLineId,
    decimal ReceivedQuantity, decimal? UnitCost = null, Guid? TaxCodeId = null);

public sealed record PurchaseOrderLineResponse(Guid Id, Guid ProductId, decimal OrderedQuantity,
    decimal ReceivedQuantity, decimal UnitPurchaseCost, Guid TaxCodeId, decimal TaxRate,
    decimal DiscountAmount, decimal LineSubTotal, decimal TaxAmount, decimal LineTotal);

public sealed record PurchaseOrderDetailsResponse(Guid Id, Guid SupplierId, Guid WarehouseId,
    DateOnly OrderDate, DateOnly? ExpectedDate, string Status, decimal SubTotal, decimal DiscountAmount,
    decimal TaxAmount, decimal GrandTotal, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt,
    string RowVersion, IReadOnlyCollection<PurchaseOrderLineResponse> Lines);

public sealed record PurchaseOrderSummaryResponse(Guid Id, Guid SupplierId, Guid WarehouseId,
    DateOnly OrderDate, DateOnly? ExpectedDate, string Status, decimal GrandTotal);

public sealed record GoodsReceiptNoteLineResponse(Guid Id, Guid? PurchaseOrderLineId, Guid ProductId,
    decimal ReceivedQuantity, decimal UnitCost, Guid TaxCodeId, decimal TaxRate,
    decimal LineSubTotal, decimal TaxAmount, decimal LineTotal);

public sealed record GoodsReceiptNoteDetailsResponse(Guid Id, Guid? PurchaseOrderId, Guid SupplierId,
    Guid WarehouseId, DateOnly ReceiptDate, string Status, DateTimeOffset? PostedAt,
    Guid? PostedByUserId, DateTimeOffset? CancelledAt, Guid? CancelledByUserId,
    DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, string RowVersion,
    IReadOnlyCollection<GoodsReceiptNoteLineResponse> Lines);

public sealed record GoodsReceiptNoteSummaryResponse(Guid Id, Guid? PurchaseOrderId, Guid SupplierId,
    Guid WarehouseId, DateOnly ReceiptDate, string Status, int LineCount);

public static class PurchasingResponseMapper
{
    public static PurchaseOrderDetailsResponse Map(PurchaseOrder order) => new(
        order.Id, order.SupplierId, order.WarehouseId, order.OrderDate, order.ExpectedDate,
        order.Status.ToString(), order.SubTotal, order.DiscountAmount, order.TaxAmount, order.GrandTotal,
        order.CreatedAt, order.UpdatedAt, Convert.ToBase64String(order.RowVersion),
        order.Lines.OrderBy(x => x.Id).Select(x => new PurchaseOrderLineResponse(x.Id, x.ProductId,
            x.OrderedQuantity, x.ReceivedQuantity, x.UnitPurchaseCost, x.TaxCodeId, x.TaxRate,
            x.DiscountAmount, x.LineSubTotal, x.TaxAmount, x.LineTotal)).ToArray());

    public static GoodsReceiptNoteDetailsResponse Map(GoodsReceiptNote note) => new(
        note.Id, note.PurchaseOrderId, note.SupplierId, note.WarehouseId, note.ReceiptDate,
        note.Status.ToString(), note.PostedAt, note.PostedByUserId, note.CancelledAt,
        note.CancelledByUserId, note.CreatedAt, note.UpdatedAt, Convert.ToBase64String(note.RowVersion),
        note.Lines.OrderBy(x => x.Id).Select(x => new GoodsReceiptNoteLineResponse(x.Id,
            x.PurchaseOrderLineId, x.ProductId, x.ReceivedQuantity, x.UnitCost, x.TaxCodeId,
            x.TaxRate, x.LineSubTotal, x.TaxAmount, x.LineTotal)).ToArray());
}
