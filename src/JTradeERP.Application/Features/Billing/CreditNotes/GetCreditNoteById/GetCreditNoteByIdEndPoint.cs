using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.GetCreditNoteById;

public static class GetCreditNoteByIdEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapGet(
        "/credit-notes/{id:guid}",
        (Guid id,ISender s,CancellationToken ct) =>
        
            s.Send(new GetCreditNoteByIdQuery(id),ct))
        
        .RequirePermission("Billing.CreditNote",Capability.View);
}
