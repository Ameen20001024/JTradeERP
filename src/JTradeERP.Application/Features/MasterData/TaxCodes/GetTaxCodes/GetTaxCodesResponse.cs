namespace JTradeERP.Application.Features.MasterData.TaxCodes.GetTaxCodes;
public sealed record GetTaxCodesResponse(Guid Id, string Code, string Name, decimal Rate, string TaxType, bool IsActive);
