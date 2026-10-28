using MediatR;using JTradeERP.Application.Features.Billing.Common;
namespace JTradeERP.Application.Features.Billing.Quotations.SendQuotation;
public sealed record SendQuotationCommand(Guid Id):IRequest<SendQuotationResponse>;
