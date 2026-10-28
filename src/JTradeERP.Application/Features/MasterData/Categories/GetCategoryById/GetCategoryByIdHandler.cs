using MediatR; using JTradeERP.Application.Common.Persistence; using JTradeERP.Application.Common.Exceptions;
namespace JTradeERP.Application.Features.MasterData.Categories.GetCategoryById;
public sealed class GetCategoryByIdHandler(IApplicationDbContext db) : IRequestHandler<GetCategoryByIdQuery, GetCategoryByIdResponse>
{ public Task<GetCategoryByIdResponse> Handle(GetCategoryByIdQuery r, CancellationToken ct) { var e=db.Categories.SingleOrDefault(x=>x.Id==r.Id) ?? throw new NotFoundException("Category not found."); return Task.FromResult(new GetCategoryByIdResponse(e.Id,e.Name,e.IsActive)); } }
