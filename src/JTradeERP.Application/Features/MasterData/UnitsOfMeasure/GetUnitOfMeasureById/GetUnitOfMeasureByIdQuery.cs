using MediatR; namespace JTradeERP.Application.Features.MasterData.UnitsOfMeasure.GetUnitOfMeasureById; public sealed record GetUnitOfMeasureByIdQuery(Guid Id):IRequest<GetUnitOfMeasureByIdResponse>;
