using MediatR;
namespace JTradeERP.Application.Features.MasterData.Categories.GetCategoryById;
public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<GetCategoryByIdResponse>;
