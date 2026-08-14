using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureTestPanels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestPanelItems_Tests_TestId",
                table: "TestPanelItems");

            migrationBuilder.DropIndex(
                name: "IX_TestPanelItems_TestPanelId",
                table: "TestPanelItems");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestPanels",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestPanels",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "TestPanels",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_TestPanels_Code",
                table: "TestPanels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestPanelItems_TestPanelId_SortOrder",
                table: "TestPanelItems",
                columns: new[] { "TestPanelId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TestPanelItems_TestPanelId_TestId",
                table: "TestPanelItems",
                columns: new[] { "TestPanelId", "TestId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TestPanelItems_Tests_TestId",
                table: "TestPanelItems",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestPanelItems_Tests_TestId",
                table: "TestPanelItems");

            migrationBuilder.DropIndex(
                name: "IX_TestPanels_Code",
                table: "TestPanels");

            migrationBuilder.DropIndex(
                name: "IX_TestPanelItems_TestPanelId_SortOrder",
                table: "TestPanelItems");

            migrationBuilder.DropIndex(
                name: "IX_TestPanelItems_TestPanelId_TestId",
                table: "TestPanelItems");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestPanels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TestPanels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "TestPanels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_TestPanelItems_TestPanelId",
                table: "TestPanelItems",
                column: "TestPanelId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestPanelItems_Tests_TestId",
                table: "TestPanelItems",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
