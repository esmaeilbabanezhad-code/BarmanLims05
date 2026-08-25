using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMatrixToSample : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matrices_SampleCategories_SampleCategoryId",
                table: "Matrices");

            migrationBuilder.DropIndex(
                name: "IX_Matrices_SampleCategoryId",
                table: "Matrices");

            migrationBuilder.DropColumn(
                name: "Matrix",
                table: "Samples");

            migrationBuilder.AddColumn<Guid>(
                name: "MatrixId",
                table: "Samples",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Matrices",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Matrices",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Matrices",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_MatrixId",
                table: "Samples",
                column: "MatrixId");

            migrationBuilder.CreateIndex(
                name: "IX_Matrices_SampleCategoryId_Code",
                table: "Matrices",
                columns: new[] { "SampleCategoryId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matrices_SampleCategoryId_Name",
                table: "Matrices",
                columns: new[] { "SampleCategoryId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrices_SampleCategories_SampleCategoryId",
                table: "Matrices",
                column: "SampleCategoryId",
                principalTable: "SampleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_Matrices_MatrixId",
                table: "Samples",
                column: "MatrixId",
                principalTable: "Matrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matrices_SampleCategories_SampleCategoryId",
                table: "Matrices");

            migrationBuilder.DropForeignKey(
                name: "FK_Samples_Matrices_MatrixId",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_MatrixId",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_Matrices_SampleCategoryId_Code",
                table: "Matrices");

            migrationBuilder.DropIndex(
                name: "IX_Matrices_SampleCategoryId_Name",
                table: "Matrices");

            migrationBuilder.DropColumn(
                name: "MatrixId",
                table: "Samples");

            migrationBuilder.AddColumn<string>(
                name: "Matrix",
                table: "Samples",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Matrices",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Matrices",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Matrices",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Matrices_SampleCategoryId",
                table: "Matrices",
                column: "SampleCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matrices_SampleCategories_SampleCategoryId",
                table: "Matrices",
                column: "SampleCategoryId",
                principalTable: "SampleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
