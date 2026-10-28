using JTradeERP.Application.Features.Purchasing.Common;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNotes;

public sealed record GetGoodsReceiptNotesResponse(IReadOnlyCollection<GoodsReceiptNoteSummaryResponse> Items,
    int PageNumber, int PageSize, int TotalCount);
