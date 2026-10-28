using MediatR;
namespace JTradeERP.Application.Features.MasterData.UnitsOfMeasure.UpdateUnitOfMeasure;
public sealed record UpdateUnitOfMeasureCommand(Guid Id,string Code,string Name) : IRequest<UpdateUnitOfMeasureResponse>;
