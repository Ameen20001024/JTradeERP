using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.GetQuotationById;

public sealed record GetQuotationByIdQuery(Guid Id):IRequest<GetQuotationByIdResponse>;
