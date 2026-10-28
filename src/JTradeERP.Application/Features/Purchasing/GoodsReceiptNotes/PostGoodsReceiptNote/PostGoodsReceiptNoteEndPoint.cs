using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.PostGoodsReceiptNote;

public static class PostGoodsReceiptNoteEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapPost("/goods-receipt-notes/{id:guid}/post", async (Guid id, ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new PostGoodsReceiptNoteCommand(id), ct)))
        .RequirePermission("Purchasing.GRN", Capability.Full);
}
