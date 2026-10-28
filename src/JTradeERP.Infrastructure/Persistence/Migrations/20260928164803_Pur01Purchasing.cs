using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTradeERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Pur01Purchasing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.CheckConstraint("CK_PurchaseOrders_Status", "[Status] IN (1,2,3,4,5)");
                    table.CheckConstraint("CK_PurchaseOrders_Totals", "[SubTotal] >= 0 AND [DiscountAmount] >= 0 AND [DiscountAmount] <= [SubTotal] AND [TaxAmount] >= 0 AND [GrandTotal] = [SubTotal] - [DiscountAmount] + [TaxAmount]");
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchasingConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowOverReceipt = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PostedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CancelledByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptNotes", x => x.Id);
                    table.CheckConstraint("CK_GoodsReceiptNotes_Status", "[Status] IN (1,2,3)");
                    table.ForeignKey(
                        name: "FK_GoodsReceiptNotes_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptNotes_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptNotes_Users_CancelledByUserId",
                        column: x => x.CancelledByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptNotes_Users_PostedByUserId",
                        column: x => x.PostedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptNotes_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderedQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitPurchaseCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TaxCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineSubTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderLines", x => x.Id);
                    table.CheckConstraint("CK_PurchaseOrderLines_CostTaxDiscount", "[UnitPurchaseCost] >= 0 AND [TaxRate] >= 0 AND [TaxRate] <= 100 AND [DiscountAmount] >= 0 AND [DiscountAmount] <= [LineSubTotal]");
                    table.CheckConstraint("CK_PurchaseOrderLines_LineTotal", "[LineTotal] = [LineSubTotal] - [DiscountAmount] + [TaxAmount]");
                    table.CheckConstraint("CK_PurchaseOrderLines_Quantities", "[OrderedQuantity] > 0 AND [ReceivedQuantity] >= 0");
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_TaxCodes_TaxCodeId",
                        column: x => x.TaxCodeId,
                        principalTable: "TaxCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptNoteLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoodsReceiptNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseOrderLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TaxCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    LineSubTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptNoteLines", x => x.Id);
                    table.CheckConstraint("CK_GoodsReceiptNoteLines_LineTotal", "[LineTotal] = [LineSubTotal] + [TaxAmount]");
                    table.CheckConstraint("CK_GoodsReceiptNoteLines_QuantityCost", "[ReceivedQuantity] > 0 AND [UnitCost] >= 0");
                    table.CheckConstraint("CK_GoodsReceiptNoteLines_Tax", "[TaxRate] >= 0 AND [TaxRate] <= 100 AND [TaxAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_GoodsReceiptNoteLines_GoodsReceiptNotes_GoodsReceiptNoteId",
                        column: x => x.GoodsReceiptNoteId,
                        principalTable: "GoodsReceiptNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptNoteLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptNoteLines_PurchaseOrderLines_PurchaseOrderLineId",
                        column: x => x.PurchaseOrderLineId,
                        principalTable: "PurchaseOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptNoteLines_TaxCodes_TaxCodeId",
                        column: x => x.TaxCodeId,
                        principalTable: "TaxCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "PurchasingConfigurations",
                columns: new[] { "Id", "AllowOverReceipt" },
                values: new object[] { new Guid("c6bb501f-46f1-44fa-8e11-7a97188dcb41"), false });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptNoteLines_GoodsReceiptNoteId",
                table: "GoodsReceiptNoteLines",
                column: "GoodsReceiptNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptNoteLines_ProductId",
                table: "GoodsReceiptNoteLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptNoteLines_PurchaseOrderLineId",
                table: "GoodsReceiptNoteLines",
                column: "PurchaseOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptNoteLines_TaxCodeId",
                table: "GoodsReceiptNoteLines",
                column: "TaxCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptNotes_CancelledByUserId",
                table: "GoodsReceiptNotes",
                column: "CancelledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptNotes_PostedByUserId",
                table: "GoodsReceiptNotes",
                column: "PostedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptNotes_PurchaseOrderId_ReceiptDate",
                table: "GoodsReceiptNotes",
                columns: new[] { "PurchaseOrderId", "ReceiptDate" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptNotes_Status_ReceiptDate",
                table: "GoodsReceiptNotes",
                columns: new[] { "Status", "ReceiptDate" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptNotes_SupplierId_ReceiptDate",
                table: "GoodsReceiptNotes",
                columns: new[] { "SupplierId", "ReceiptDate" });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptNotes_WarehouseId",
                table: "GoodsReceiptNotes",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_ProductId",
                table: "PurchaseOrderLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_PurchaseOrderId",
                table: "PurchaseOrderLines",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_TaxCodeId",
                table: "PurchaseOrderLines",
                column: "TaxCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CreatedByUserId",
                table: "PurchaseOrders",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_Status_OrderDate",
                table: "PurchaseOrders",
                columns: new[] { "Status", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SupplierId",
                table: "PurchaseOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_UpdatedByUserId",
                table: "PurchaseOrders",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_WarehouseId",
                table: "PurchaseOrders",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsReceiptNoteLines");

            migrationBuilder.DropTable(
                name: "PurchasingConfigurations");

            migrationBuilder.DropTable(
                name: "GoodsReceiptNotes");

            migrationBuilder.DropTable(
                name: "PurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");
        }
    }
}
