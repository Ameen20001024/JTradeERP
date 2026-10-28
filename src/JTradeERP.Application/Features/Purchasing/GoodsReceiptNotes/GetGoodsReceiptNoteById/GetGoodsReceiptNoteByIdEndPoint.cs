using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNoteById;

public static class GetGoodsReceiptNoteByIdEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapGet("/goods-receipt-notes/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new GetGoodsReceiptNoteByIdQuery(id), ct)))
        .RequirePermission("Purchasing.GRN", Capability.View);
}
