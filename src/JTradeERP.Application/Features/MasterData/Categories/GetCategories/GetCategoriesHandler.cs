using MediatR; using JTradeERP.Application.Common.Persistence;
namespace JTradeERP.Application.Features.MasterData.Categories.GetCategories;
public sealed class GetCategoriesHandler(IApplicationDbContext db) : IRequestHandler<GetCategoriesQuery, IReadOnlyList<GetCategoriesResponse>>
{ public Task<IReadOnlyList<GetCategoriesResponse>> Handle(GetCategoriesQuery r, CancellationToken ct) => Task.FromResult<IReadOnlyList<GetCategoriesResponse>>(db.Categories.OrderBy(x=>x.Name).Select(x=>new GetCategoriesResponse(x.Id,x.Name,x.IsActive)).ToList()); }
