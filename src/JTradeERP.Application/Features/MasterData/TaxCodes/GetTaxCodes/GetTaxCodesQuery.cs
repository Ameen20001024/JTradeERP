using MediatR;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.GetTaxCodes;
public sealed record GetTaxCodesQuery : IRequest<IReadOnlyList<GetTaxCodesResponse>>;
