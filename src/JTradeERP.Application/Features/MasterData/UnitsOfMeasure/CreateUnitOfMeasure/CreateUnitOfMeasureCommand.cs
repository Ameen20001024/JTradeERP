using MediatR;
namespace JTradeERP.Application.Features.MasterData.UnitsOfMeasure.CreateUnitOfMeasure;
public sealed record CreateUnitOfMeasureCommand(string Code, string Name) : IRequest<CreateUnitOfMeasureResponse>;
