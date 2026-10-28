namespace JTradeERP.Application.Features.MasterData.TaxCodes.CreateTaxCode;
public sealed record CreateTaxCodeResponse(Guid Id, string Code, string Name, decimal Rate, string TaxType, bool IsActive);
