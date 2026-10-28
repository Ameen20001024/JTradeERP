using MediatR; namespace JTradeERP.Application.Features.MasterData.Categories.DeactivateCategory; public sealed record DeactivateCategoryCommand(Guid Id) : IRequest<DeactivateCategoryResponse>;
