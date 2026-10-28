namespace JTradeERP.Application.Features.MasterData.TaxCodes.GetTaxCodeById;
public sealed record GetTaxCodeByIdResponse(Guid Id, string Code, string Name, decimal Rate, string TaxType, bool IsActive);
