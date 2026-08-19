using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korp.Stock.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockDebitOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stock_debit_operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_debit_operations", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stock_debit_operations_invoice_id",
                table: "stock_debit_operations",
                column: "invoice_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_debit_operations");
        }
    }
}
