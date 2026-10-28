using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.CancelGoodsReceiptNote;

public static class CancelGoodsReceiptNoteEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapPost("/goods-receipt-notes/{id:guid}/cancel", async (Guid id, ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new CancelGoodsReceiptNoteCommand(id), ct)))
        .RequirePermission("Purchasing.GRN", Capability.Full);
}
