using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.CancelQuotation;
public sealed record CancelQuotationCommand(Guid Id):IRequest<CancelQuotationResponse>;
