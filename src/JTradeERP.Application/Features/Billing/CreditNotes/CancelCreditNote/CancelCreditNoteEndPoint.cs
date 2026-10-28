using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.CancelCreditNote;

public static class CancelCreditNoteEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapPost(
            "/credit-notes/{id:guid}/cancel",
            async(Guid id,ISender s,CancellationToken ct) => 
            
            Results.Ok(await s.Send(new CancelCreditNoteCommand(id), ct)))
        
        .RequirePermission("Billing.CreditNote",Capability.Post)
        .RequirePermission("Billing.Cancel",Capability.Post);
}
