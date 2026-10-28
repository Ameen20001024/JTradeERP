using JTradeERP.Application.Common.Reports;
using JTradeERP.Domain.Inventory;
using MediatR;

namespace JTradeERP.Application.Features.Reports.StockMovement;

public sealed record GetStockMovementQuery(DateTimeOffset? FromUtc = null, DateTimeOffset? ToUtc = null,
    Guid? WarehouseId = null, Guid? ProductId = null, StockMovementType? MovementType = null,
    string? SourceDocumentType = null, int Page = 1, int PageSize = 50,
    string SortBy = "postedAt", string SortDirection = "desc")
    : IRequest<PagedReportResponse<StockMovementRowResponse>>;
