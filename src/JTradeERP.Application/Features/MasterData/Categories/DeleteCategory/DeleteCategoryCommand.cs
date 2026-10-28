using MediatR; namespace JTradeERP.Application.Features.MasterData.Categories.DeleteCategory; public sealed record DeleteCategoryCommand(Guid Id) : IRequest;
