using JTradeERP.Domain.Purchasing;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNotes;

public sealed record GetGoodsReceiptNotesQuery(Guid? PurchaseOrderId = null, Guid? SupplierId = null,
    Guid? WarehouseId = null, GoodsReceiptNoteStatus? Status = null, int PageNumber = 1, int PageSize = 50)
    : IRequest<GetGoodsReceiptNotesResponse>;
