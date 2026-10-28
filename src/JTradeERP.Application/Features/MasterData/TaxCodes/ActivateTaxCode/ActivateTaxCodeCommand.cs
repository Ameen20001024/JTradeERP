using MediatR;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.ActivateTaxCode;
public sealed record ActivateTaxCodeCommand(Guid Id) : IRequest<ActivateTaxCodeResponse>;
