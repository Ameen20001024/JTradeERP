using MediatR;namespace JTradeERP.Application.Features.Billing.NumberSeries.GetNumberSeriesById;public sealed record GetNumberSeriesByIdQuery(Guid Id):IRequest<GetNumberSeriesByIdResponse>;
