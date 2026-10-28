using MediatR;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.CreateTaxCode;
public sealed record CreateTaxCodeCommand(string Code, string Name, decimal Rate, string TaxType, bool IsActive) : IRequest<CreateTaxCodeResponse>;
