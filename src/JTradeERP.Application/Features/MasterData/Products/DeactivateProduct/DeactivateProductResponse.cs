namespace JTradeERP.Application.Features.MasterData.Products.DeactivateProduct;
public sealed record DeactivateProductResponse(Guid Id,string ProductCode,string Name,bool IsActive);
