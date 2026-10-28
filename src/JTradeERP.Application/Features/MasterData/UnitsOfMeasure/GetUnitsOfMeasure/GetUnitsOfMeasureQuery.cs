using MediatR; namespace JTradeERP.Application.Features.MasterData.UnitsOfMeasure.GetUnitsOfMeasure; public sealed record GetUnitsOfMeasureQuery:IRequest<IReadOnlyList<GetUnitsOfMeasureResponse>>;
