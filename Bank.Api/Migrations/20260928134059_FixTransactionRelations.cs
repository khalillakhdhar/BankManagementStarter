using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixTransactionRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Comptes_CompteId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Comptes_CompteId1",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CompteId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CompteId1",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CompteId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CompteId1",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompteId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompteId1",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CompteId",
                table: "Transactions",
                column: "CompteId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CompteId1",
                table: "Transactions",
                column: "CompteId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Comptes_CompteId",
                table: "Transactions",
                column: "CompteId",
                principalTable: "Comptes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Comptes_CompteId1",
                table: "Transactions",
                column: "CompteId1",
                principalTable: "Comptes",
                principalColumn: "Id");
        }
    }
}
