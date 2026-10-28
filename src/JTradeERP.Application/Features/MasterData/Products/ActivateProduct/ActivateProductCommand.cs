using MediatR;
namespace JTradeERP.Application.Features.MasterData.Products.ActivateProduct;
public sealed record ActivateProductCommand(Guid Id) : IRequest<ActivateProductResponse>;
