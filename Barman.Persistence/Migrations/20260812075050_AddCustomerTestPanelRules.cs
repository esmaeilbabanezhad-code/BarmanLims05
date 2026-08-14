using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerTestPanelRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_TestPanels_DefaultTestPanelId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_DefaultTestPanelId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DefaultTestPanelId",
                table: "Customers");

            migrationBuilder.CreateTable(
                name: "CustomerTestPanels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SampleCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    MatrixId = table.Column<Guid>(type: "uuid", nullable: true),
                    TestPanelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTestPanels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerTestPanels_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerTestPanels_Matrices_MatrixId",
                        column: x => x.MatrixId,
                        principalTable: "Matrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerTestPanels_SampleCategories_SampleCategoryId",
                        column: x => x.SampleCategoryId,
                        principalTable: "SampleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerTestPanels_TestPanels_TestPanelId",
                        column: x => x.TestPanelId,
                        principalTable: "TestPanels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTestPanels_CustomerId_SampleCategoryId_MatrixId_Pri~",
                table: "CustomerTestPanels",
                columns: new[] { "CustomerId", "SampleCategoryId", "MatrixId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTestPanels_MatrixId",
                table: "CustomerTestPanels",
                column: "MatrixId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTestPanels_SampleCategoryId",
                table: "CustomerTestPanels",
                column: "SampleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTestPanels_TestPanelId",
                table: "CustomerTestPanels",
                column: "TestPanelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerTestPanels");

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultTestPanelId",
                table: "Customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DefaultTestPanelId",
                table: "Customers",
                column: "DefaultTestPanelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_TestPanels_DefaultTestPanelId",
                table: "Customers",
                column: "DefaultTestPanelId",
                principalTable: "TestPanels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
