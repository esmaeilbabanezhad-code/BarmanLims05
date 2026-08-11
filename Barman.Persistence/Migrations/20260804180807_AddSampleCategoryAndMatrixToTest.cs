using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleCategoryAndMatrixToTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MatrixId",
                table: "Tests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SampleCategoryId",
                table: "Tests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_MatrixId",
                table: "Tests",
                column: "MatrixId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_SampleCategoryId",
                table: "Tests",
                column: "SampleCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Matrices_MatrixId",
                table: "Tests",
                column: "MatrixId",
                principalTable: "Matrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_SampleCategories_SampleCategoryId",
                table: "Tests",
                column: "SampleCategoryId",
                principalTable: "SampleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Matrices_MatrixId",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_SampleCategories_SampleCategoryId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_MatrixId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_SampleCategoryId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "MatrixId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "SampleCategoryId",
                table: "Tests");
        }
    }
}
