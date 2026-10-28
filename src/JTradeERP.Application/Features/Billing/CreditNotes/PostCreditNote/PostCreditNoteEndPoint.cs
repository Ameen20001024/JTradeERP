using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.PostCreditNote;

public static class PostCreditNoteEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapPost(
        "/credit-notes/{id:guid}/post",
        async(Guid id,ISender s,CancellationToken ct) => 
        
            Results.Ok(await s.Send(new PostCreditNoteCommand(id),ct)))
        
        .RequirePermission("Billing.CreditNote",Capability.Post)
        .RequirePermission("Billing.Post",Capability.Post);
}
