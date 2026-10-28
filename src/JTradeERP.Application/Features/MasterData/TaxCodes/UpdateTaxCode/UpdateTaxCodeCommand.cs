using MediatR;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.UpdateTaxCode;
public sealed record UpdateTaxCodeCommand(Guid Id, string Code, string Name, decimal Rate, string TaxType) : IRequest<UpdateTaxCodeResponse>;
