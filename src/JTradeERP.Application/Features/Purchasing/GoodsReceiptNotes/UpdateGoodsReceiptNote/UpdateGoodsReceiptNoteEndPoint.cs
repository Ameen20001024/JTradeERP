using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.UpdateGoodsReceiptNote;

public static class UpdateGoodsReceiptNoteEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapPut("/goods-receipt-notes/{id:guid}", (Guid id, UpdateGoodsReceiptNoteCommand command, ISender sender, CancellationToken ct) =>
            sender.Send(command with { Id = id }, ct))
        .RequirePermission("Purchasing.GRN", Capability.CreateEdit);
}
