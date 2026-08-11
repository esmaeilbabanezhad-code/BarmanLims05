using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SampleCategoryId",
                table: "Samples",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SampleCategories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SampleCategories",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SampleCategories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_SampleCategoryId",
                table: "Samples",
                column: "SampleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleCategories_Code",
                table: "SampleCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SampleCategories_Name",
                table: "SampleCategories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_SampleCategories_SampleCategoryId",
                table: "Samples",
                column: "SampleCategoryId",
                principalTable: "SampleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_SampleCategories_SampleCategoryId",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_SampleCategoryId",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_SampleCategories_Code",
                table: "SampleCategories");

            migrationBuilder.DropIndex(
                name: "IX_SampleCategories_Name",
                table: "SampleCategories");

            migrationBuilder.DropColumn(
                name: "SampleCategoryId",
                table: "Samples");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SampleCategories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SampleCategories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SampleCategories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
