using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Billing;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.CreditNotes.GetCreditNotes;

public static class GetCreditNotesEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapGet(
        "/credit-notes",
        (Guid? invoiceId,
            CreditNoteStatus? status,
            int page,
            int pageSize,
            ISender s,
            CancellationToken ct) =>
        
        s.Send(new GetCreditNotesQuery(
            invoiceId,
            status,
            page==0?1:page,
            pageSize==0?50:pageSize),
            ct))
        .RequirePermission("Billing.CreditNote",Capability.View);
}
