namespace JTradeERP.Application.Features.MasterData.TaxCodes.DeactivateTaxCode;
public sealed record DeactivateTaxCodeResponse(Guid Id, string Code, string Name, decimal Rate, string TaxType, bool IsActive);
