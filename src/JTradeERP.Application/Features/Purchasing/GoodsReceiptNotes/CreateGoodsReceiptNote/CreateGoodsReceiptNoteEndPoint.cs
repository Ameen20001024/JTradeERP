using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.CreateGoodsReceiptNote;

public static class CreateGoodsReceiptNoteEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapPost("/goods-receipt-notes", async (CreateGoodsReceiptNoteCommand command, ISender sender, CancellationToken ct) =>
        {
            var response = await sender.Send(command, ct);
            return Results.Created($"/api/purchasing/goods-receipt-notes/{response.GoodsReceiptNote.Id}", response);
        })
        .RequirePermission("Purchasing.GRN", Capability.CreateEdit);
}
