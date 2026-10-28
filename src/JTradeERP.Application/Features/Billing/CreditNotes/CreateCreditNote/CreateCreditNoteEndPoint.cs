using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.CreateCreditNote;

public static class CreateCreditNoteEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapPost(
        "/credit-notes",
        async(CreateCreditNoteCommand c,ISender s,CancellationToken ct) => 
        
            Results.Ok(await s.Send(c,ct)))
        
        .RequirePermission("Billing.CreditNote",Capability.CreateEdit);
}
