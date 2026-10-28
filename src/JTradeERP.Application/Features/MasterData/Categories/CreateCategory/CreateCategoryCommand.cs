using MediatR;
namespace JTradeERP.Application.Features.MasterData.Categories.CreateCategory;
public sealed record CreateCategoryCommand(string Name) : IRequest<CreateCategoryResponse>;
