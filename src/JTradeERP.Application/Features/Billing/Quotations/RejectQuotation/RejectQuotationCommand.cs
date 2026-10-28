using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.RejectQuotation;

public sealed record RejectQuotationCommand(Guid Id):IRequest<RejectQuotationResponse>;
