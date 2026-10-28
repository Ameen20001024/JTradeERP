using MediatR;
namespace JTradeERP.Application.Features.MasterData.Products.GetProducts;
public sealed record GetProductsQuery : IRequest<IReadOnlyList<GetProductsResponse>>;
