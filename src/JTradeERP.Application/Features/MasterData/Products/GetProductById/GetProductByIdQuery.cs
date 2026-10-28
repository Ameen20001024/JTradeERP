using MediatR;
namespace JTradeERP.Application.Features.MasterData.Products.GetProductById;
public sealed record GetProductByIdQuery(Guid Id) : IRequest<GetProductByIdResponse>;
