namespace JTradeERP.Application.Features.MasterData.TaxCodes.ActivateTaxCode;
public sealed record ActivateTaxCodeResponse(Guid Id, string Code, string Name, decimal Rate, string TaxType, bool IsActive);
