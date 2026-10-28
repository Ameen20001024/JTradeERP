using MediatR;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.GetTaxCodeById;
public sealed record GetTaxCodeByIdQuery(Guid Id) : IRequest<GetTaxCodeByIdResponse>;
