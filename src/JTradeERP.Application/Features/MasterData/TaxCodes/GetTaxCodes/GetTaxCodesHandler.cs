using MediatR; using JTradeERP.Application.Common.Persistence;
namespace JTradeERP.Application.Features.MasterData.TaxCodes.GetTaxCodes;
public sealed class GetTaxCodesHandler(IApplicationDbContext db) : IRequestHandler<GetTaxCodesQuery,IReadOnlyList<GetTaxCodesResponse>> { public Task<IReadOnlyList<GetTaxCodesResponse>> Handle(GetTaxCodesQuery r,CancellationToken ct) => Task.FromResult<IReadOnlyList<GetTaxCodesResponse>>(db.TaxCodes.OrderBy(x=>x.Code).Select(x=>new GetTaxCodesResponse(x.Id,x.Code,x.Name,x.Rate,x.TaxType,x.IsActive)).ToList()); }
