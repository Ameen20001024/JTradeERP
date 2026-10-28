using MediatR;
namespace JTradeERP.Application.Features.MasterData.Products.DeactivateProduct;
public sealed record DeactivateProductCommand(Guid Id) : IRequest<DeactivateProductResponse>;
