namespace JTradeERP.Application.Features.MasterData.TaxCodes.UpdateTaxCode;
public sealed record UpdateTaxCodeResponse(Guid Id, string Code, string Name, decimal Rate, string TaxType, bool IsActive);
