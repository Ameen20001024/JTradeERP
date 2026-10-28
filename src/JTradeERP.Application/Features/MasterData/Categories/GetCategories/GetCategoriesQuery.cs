using MediatR;
namespace JTradeERP.Application.Features.MasterData.Categories.GetCategories;
public sealed record GetCategoriesQuery : IRequest<IReadOnlyList<GetCategoriesResponse>>;
