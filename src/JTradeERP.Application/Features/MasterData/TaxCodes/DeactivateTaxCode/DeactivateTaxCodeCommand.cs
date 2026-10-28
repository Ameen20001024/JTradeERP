using MediatR;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.DeactivateTaxCode;
public sealed record DeactivateTaxCodeCommand(Guid Id) : IRequest<DeactivateTaxCodeResponse>;
