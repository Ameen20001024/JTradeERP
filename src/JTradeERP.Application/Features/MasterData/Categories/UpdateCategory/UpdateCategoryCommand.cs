using MediatR;
namespace JTradeERP.Application.Features.MasterData.Categories.UpdateCategory;
public sealed record UpdateCategoryCommand(Guid Id, string Name) : IRequest<UpdateCategoryResponse>;
