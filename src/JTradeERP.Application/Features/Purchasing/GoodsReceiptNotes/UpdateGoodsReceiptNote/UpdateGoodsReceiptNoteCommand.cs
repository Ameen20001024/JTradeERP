using JTradeERP.Application.Features.Purchasing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.UpdateGoodsReceiptNote;

public sealed record UpdateGoodsReceiptNoteCommand(Guid Id, Guid? PurchaseOrderId, Guid SupplierId,
    Guid WarehouseId, DateOnly ReceiptDate, IReadOnlyCollection<GoodsReceiptNoteLineInput> Lines)
    : IRequest<UpdateGoodsReceiptNoteResponse>;
