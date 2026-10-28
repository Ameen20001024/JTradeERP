using MediatR; using JTradeERP.Application.Common.Persistence; using JTradeERP.Application.Common.Exceptions; using JTradeERP.Domain.MasterData;
namespace JTradeERP.Application.Features.MasterData.Categories.CreateCategory;
public sealed class CreateCategoryHandler(IApplicationDbContext db) : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{ public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand r, CancellationToken ct) { if (db.Categories.Any(x => x.Name == r.Name.Trim())) throw new ConflictException("Category name already exists."); var e = new Category(r.Name); db.Add(e); await db.SaveChangesAsync(ct); return new(e.Id,e.Name,e.IsActive); } }
