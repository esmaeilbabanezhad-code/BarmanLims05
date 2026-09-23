using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMatrixToTestTariff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MatrixId",
                table: "TestTariffs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestTariffs_MatrixId",
                table: "TestTariffs",
                column: "MatrixId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTariffs_Matrices_MatrixId",
                table: "TestTariffs",
                column: "MatrixId",
                principalTable: "Matrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestTariffs_Matrices_MatrixId",
                table: "TestTariffs");

            migrationBuilder.DropIndex(
                name: "IX_TestTariffs_MatrixId",
                table: "TestTariffs");

            migrationBuilder.DropColumn(
                name: "MatrixId",
                table: "TestTariffs");
        }
    }
}
