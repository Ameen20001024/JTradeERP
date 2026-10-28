using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNotes;

public static class GetGoodsReceiptNotesEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapGet("/goods-receipt-notes", async ([AsParameters] GetGoodsReceiptNotesQuery query, ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(query, ct)))
        .RequirePermission("Purchasing.GRN", Capability.View);
}
