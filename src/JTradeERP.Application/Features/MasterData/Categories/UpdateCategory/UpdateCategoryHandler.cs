using MediatR; using JTradeERP.Application.Common.Persistence; using JTradeERP.Application.Common.Exceptions;
namespace JTradeERP.Application.Features.MasterData.Categories.UpdateCategory;
public sealed class UpdateCategoryHandler(IApplicationDbContext db) : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
{ public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand r, CancellationToken ct) { var e=db.Categories.SingleOrDefault(x=>x.Id==r.Id) ?? throw new NotFoundException("Category not found."); if(db.Categories.Any(x=>x.Id!=r.Id && x.Name==r.Name.Trim())) throw new ConflictException("Category name already exists."); e.Rename(r.Name); await db.SaveChangesAsync(ct); return new(e.Id,e.Name,e.IsActive); } }
