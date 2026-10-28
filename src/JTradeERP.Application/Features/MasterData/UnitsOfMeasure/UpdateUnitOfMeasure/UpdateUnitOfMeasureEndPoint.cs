using MediatR; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.UnitsOfMeasure.UpdateUnitOfMeasure;
public static class UpdateUnitOfMeasureEndpoint { public static void Map(IEndpointRouteBuilder e)=>e.MapPut("/units-of-measure/{id:guid}",async(Guid id,UpdateUnitOfMeasureCommand command,ISender s,CancellationToken ct)=>Results.Ok(await s.Send(command with { Id=id },ct))); }
