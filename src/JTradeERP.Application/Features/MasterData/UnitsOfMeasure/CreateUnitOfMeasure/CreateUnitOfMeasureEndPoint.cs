using MediatR; using Microsoft.AspNetCore.Http; using Microsoft.AspNetCore.Routing;
namespace JTradeERP.Application.Features.MasterData.UnitsOfMeasure.CreateUnitOfMeasure;
public static class CreateUnitOfMeasureEndpoint { public static void Map(IEndpointRouteBuilder e)=>e.MapPost("/units-of-measure",async(CreateUnitOfMeasureCommand command,ISender s,CancellationToken ct)=>{var response=await s.Send(command,ct);return Results.Created($"/api/master-data/units-of-measure/{response.Id}",response);}); }
