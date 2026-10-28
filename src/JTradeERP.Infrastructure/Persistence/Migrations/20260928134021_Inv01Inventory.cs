using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTradeERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inv01Inventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowNegativeStock",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "OpeningBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PostedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReversedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReversedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_Users_PostedByUserId",
                        column: x => x.PostedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpeningBalances_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockAdjustments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuantityDelta = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ReasonCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PostedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReversedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReversedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Users_PostedByUserId",
                        column: x => x.PostedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Users_ReversedByUserId",
                        column: x => x.ReversedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockBalances",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QtyOnHand = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    QtyReserved = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WeightedAverageCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockBalances", x => new { x.ProductId, x.WarehouseId });
                    table.CheckConstraint("CK_StockBalances_QtyReserved_NonNegative", "[QtyReserved] >= 0");
                    table.ForeignKey(
                        name: "FK_StockBalances_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockBalances_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockLedgerEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovementType = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SourceDocumentType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReversalOfLedgerEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PostedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLedgerEntries", x => x.Id);
                    table.CheckConstraint("CK_StockLedger_Direction", "[Direction] IN (1, 2)");
                    table.CheckConstraint("CK_StockLedger_MovementType", "[MovementType] IN (1, 2, 3, 4, 5, 6)");
                    table.CheckConstraint("CK_StockLedger_Quantity_Positive", "[Quantity] > 0");
                    table.CheckConstraint("CK_StockLedger_UnitCost_NonNegative", "[UnitCost] >= 0");
                    table.ForeignKey(
                        name: "FK_StockLedgerEntries_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLedgerEntries_StockLedgerEntries_ReversalOfLedgerEntryId",
                        column: x => x.ReversalOfLedgerEntryId,
                        principalTable: "StockLedgerEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLedgerEntries_Users_PostedByUserId",
                        column: x => x.PostedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLedgerEntries_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTransfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PostedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransfers_Users_PostedByUserId",
                        column: x => x.PostedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransfers_Warehouses_DestinationWarehouseId",
                        column: x => x.DestinationWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransfers_Warehouses_SourceWarehouseId",
                        column: x => x.SourceWarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTransferLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockTransferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransferLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransferLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransferLines_StockTransfers_StockTransferId",
                        column: x => x.StockTransferId,
                        principalTable: "StockTransfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_PostedByUserId",
                table: "OpeningBalances",
                column: "PostedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_ProductId",
                table: "OpeningBalances",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_SourceDocumentId",
                table: "OpeningBalances",
                column: "SourceDocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpeningBalances_WarehouseId",
                table: "OpeningBalances",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_PostedByUserId",
                table: "StockAdjustments",
                column: "PostedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_ProductId_WarehouseId_Status",
                table: "StockAdjustments",
                columns: new[] { "ProductId", "WarehouseId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_ReversedByUserId",
                table: "StockAdjustments",
                column: "ReversedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_WarehouseId",
                table: "StockAdjustments",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalances_WarehouseId",
                table: "StockBalances",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgerEntries_PostedByUserId",
                table: "StockLedgerEntries",
                column: "PostedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgerEntries_ProductId_WarehouseId_PostedAt",
                table: "StockLedgerEntries",
                columns: new[] { "ProductId", "WarehouseId", "PostedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgerEntries_ReversalOfLedgerEntryId",
                table: "StockLedgerEntries",
                column: "ReversalOfLedgerEntryId",
                unique: true,
                filter: "[ReversalOfLedgerEntryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgerEntries_SourceDocumentType_SourceDocumentId",
                table: "StockLedgerEntries",
                columns: new[] { "SourceDocumentType", "SourceDocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgerEntries_WarehouseId",
                table: "StockLedgerEntries",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "UX_StockLedger_SourceEffect",
                table: "StockLedgerEntries",
                columns: new[] { "SourceDocumentType", "SourceDocumentId", "SourceLineId", "MovementType", "Direction" },
                unique: true,
                filter: "[SourceLineId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferLines_ProductId",
                table: "StockTransferLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferLines_StockTransferId_ProductId",
                table: "StockTransferLines",
                columns: new[] { "StockTransferId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_DestinationWarehouseId",
                table: "StockTransfers",
                column: "DestinationWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_PostedByUserId",
                table: "StockTransfers",
                column: "PostedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_SourceWarehouseId",
                table: "StockTransfers",
                column: "SourceWarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpeningBalances");

            migrationBuilder.DropTable(
                name: "StockAdjustments");

            migrationBuilder.DropTable(
                name: "StockBalances");

            migrationBuilder.DropTable(
                name: "StockLedgerEntries");

            migrationBuilder.DropTable(
                name: "StockTransferLines");

            migrationBuilder.DropTable(
                name: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "AllowNegativeStock",
                table: "Warehouses");
        }
    }
}
