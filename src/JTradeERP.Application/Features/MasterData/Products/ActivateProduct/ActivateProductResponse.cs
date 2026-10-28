namespace JTradeERP.Application.Features.MasterData.Products.ActivateProduct;
public sealed record ActivateProductResponse(Guid Id,string ProductCode,string Name,bool IsActive);
