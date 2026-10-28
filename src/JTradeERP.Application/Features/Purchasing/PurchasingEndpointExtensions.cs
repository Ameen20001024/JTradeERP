using JTradeERP.Application.Features.Purchasing.Configuration.GetPurchasingConfiguration;
using JTradeERP.Application.Features.Purchasing.Configuration.UpdatePurchasingConfiguration;
using JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.CancelGoodsReceiptNote;
using JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.CreateGoodsReceiptNote;
using JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNoteById;
using JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.GetGoodsReceiptNotes;
using JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.PostGoodsReceiptNote;
using JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.UpdateGoodsReceiptNote;
using JTradeERP.Application.Features.Purchasing.PurchaseOrders.CreatePurchaseOrder;
using JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrderById;
using JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrders;
using JTradeERP.Application.Features.Purchasing.PurchaseOrders.UpdatePurchaseOrder;

namespace JTradeERP.Application.Features.Purchasing;

public static class PurchasingEndpointExtensions
{
    public static IEndpointRouteBuilder MapPurchasingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/purchasing").RequireAuthorization();
        CreatePurchaseOrderEndPoint.Map(group);
        UpdatePurchaseOrderEndPoint.Map(group);
        GetPurchaseOrderByIdEndPoint.Map(group);
        GetPurchaseOrdersEndPoint.Map(group);
        CreateGoodsReceiptNoteEndPoint.Map(group);
        UpdateGoodsReceiptNoteEndPoint.Map(group);
        PostGoodsReceiptNoteEndPoint.Map(group);
        CancelGoodsReceiptNoteEndPoint.Map(group);
        GetGoodsReceiptNoteByIdEndPoint.Map(group);
        GetGoodsReceiptNotesEndPoint.Map(group);
        GetPurchasingConfigurationEndPoint.Map(group);
        UpdatePurchasingConfigurationEndPoint.Map(group);
        return endpoints;
    }
}
