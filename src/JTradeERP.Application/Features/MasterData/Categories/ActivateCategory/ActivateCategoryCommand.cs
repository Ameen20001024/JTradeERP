using MediatR; namespace JTradeERP.Application.Features.MasterData.Categories.ActivateCategory; public sealed record ActivateCategoryCommand(Guid Id) : IRequest<ActivateCategoryResponse>;
