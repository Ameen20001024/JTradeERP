using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.UpdateCreditNote;

public static class UpdateCreditNoteEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapPut(
        "/credit-notes/{id:guid}",
        async (Guid id,UpdateCreditNoteCommand c,ISender s,CancellationToken ct) => 
        
        Results.Ok(await s.Send(c with{Id=id},ct)))
        
        .RequirePermission("Billing.CreditNote",Capability.CreateEdit);
}
