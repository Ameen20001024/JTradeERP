using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Domain.MasterData;
using JTradeERP.Domain.Purchasing;

namespace JTradeERP.Application.Features.Purchasing.Common;

internal static class PurchasingReferenceChecks
{
    public static void ValidateOrderRelationship(JTradeERP.Domain.Purchasing.PurchaseOrder? order,
        Guid supplierId, Guid warehouseId)
    {
        if (order is null) return;
        if (order.Status == JTradeERP.Domain.Purchasing.PurchaseOrderStatus.Cancelled)
            throw new ConflictException("A cancelled purchase order cannot receive goods.");
        if (order.SupplierId != supplierId || order.WarehouseId != warehouseId)
            throw new ConflictException("GRN supplier and warehouse must match the referenced purchase order.");
    }

    public static void EnsureSupplierAndWarehouse(IApplicationDbContext db, Guid supplierId, Guid warehouseId)
    {
        _ = db.Suppliers.SingleOrDefault(x => x.Id == supplierId)
            ?? throw new NotFoundException("Supplier was not found.");
        var warehouse = db.Warehouses.SingleOrDefault(x => x.Id == warehouseId)
            ?? throw new NotFoundException("Warehouse was not found.");
        if (!warehouse.IsActive) throw new ConflictException("Inactive warehouses cannot be used in purchasing documents.");
    }

    public static void EnsureActiveProductAndTax(IApplicationDbContext db, Guid productId, Guid taxCodeId)
    {
        var product = db.Products.SingleOrDefault(x => x.Id == productId)
            ?? throw new NotFoundException("Product was not found.");
        var taxCode = db.TaxCodes.SingleOrDefault(x => x.Id == taxCodeId)
            ?? throw new NotFoundException("Tax code was not found.");
        if (!product.IsActive) throw new ConflictException("Inactive products cannot be used in purchasing documents.");
        if (!taxCode.IsActive) throw new ConflictException("Inactive tax codes cannot be used in new purchasing documents.");
    }

    public static PurchaseOrderLine CreatePurchaseOrderLine(IApplicationDbContext db, PurchaseOrderLineInput input)
    {
        EnsureActiveProductAndTax(db, input.ProductId, input.TaxCodeId);
        var taxRate = db.TaxCodes.Single(x => x.Id == input.TaxCodeId).Rate;
        return new PurchaseOrderLine(input.ProductId, input.OrderedQuantity, input.UnitPurchaseCost,
            input.TaxCodeId, taxRate, input.DiscountAmount);
    }

    public static GoodsReceiptNoteLine CreateGoodsReceiptNoteLine(IApplicationDbContext db,
        GoodsReceiptNoteLineInput input, PurchaseOrder? order)
    {
        var purchaseOrderLine = input.PurchaseOrderLineId.HasValue
            ? order?.Lines.SingleOrDefault(x => x.Id == input.PurchaseOrderLineId.Value)
                ?? throw new ConflictException("The GRN line must reference a line belonging to its purchase order.")
            : null;
        if (order is not null && purchaseOrderLine is null)
            throw new ConflictException("Every PO-linked GRN line must reference a purchase order line.");
        if (purchaseOrderLine is not null && purchaseOrderLine.ProductId != input.ProductId)
            throw new ConflictException("The GRN product must match the referenced purchase order line.");

        var taxCodeId = input.TaxCodeId ?? purchaseOrderLine?.TaxCodeId
            ?? throw new ArgumentException("A tax code is required for a standalone GRN line.");
        EnsureActiveProductAndTax(db, input.ProductId, taxCodeId);
        if (purchaseOrderLine is not null && taxCodeId != purchaseOrderLine.TaxCodeId)
            throw new ConflictException("A PO-linked GRN must retain the purchase order line tax code.");
        var taxRate = purchaseOrderLine?.TaxRate ?? db.TaxCodes.Single(x => x.Id == taxCodeId).Rate;
        var unitCost = input.UnitCost ?? purchaseOrderLine?.UnitPurchaseCost
            ?? throw new ArgumentException("A unit cost is required for a standalone GRN line.");
        return new GoodsReceiptNoteLine(input.ProductId, input.PurchaseOrderLineId,
            input.ReceivedQuantity, unitCost, taxCodeId, taxRate);
    }
}
