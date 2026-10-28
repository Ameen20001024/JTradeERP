using JTradeERP.Application.Features.Purchasing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.CreateGoodsReceiptNote;

public sealed record CreateGoodsReceiptNoteCommand(Guid? PurchaseOrderId, Guid SupplierId,
    Guid WarehouseId, DateOnly ReceiptDate, IReadOnlyCollection<GoodsReceiptNoteLineInput> Lines)
    : IRequest<CreateGoodsReceiptNoteResponse>;
