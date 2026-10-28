using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JTradeERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inv01SourceDocumentIdempotency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UX_StockLedger_SourceEffect",
                table: "StockLedgerEntries",
                newName: "UX_StockLedger_SourceEffect_Line");

            migrationBuilder.CreateIndex(
                name: "UX_StockLedger_SourceEffect_Document",
                table: "StockLedgerEntries",
                columns: new[] { "SourceDocumentType", "SourceDocumentId", "MovementType", "Direction" },
                unique: true,
                filter: "[SourceLineId] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_StockLedger_SourceEffect_Document",
                table: "StockLedgerEntries");

            migrationBuilder.RenameIndex(
                name: "UX_StockLedger_SourceEffect_Line",
                table: "StockLedgerEntries",
                newName: "UX_StockLedger_SourceEffect");
        }
    }
}
