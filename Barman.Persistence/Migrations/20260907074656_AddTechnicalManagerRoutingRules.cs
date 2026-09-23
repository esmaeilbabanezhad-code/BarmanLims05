using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicalManagerRoutingRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechnicalManagerRoutingRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    SampleCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    MatrixId = table.Column<Guid>(type: "uuid", nullable: true),
                    TestPanelId = table.Column<Guid>(type: "uuid", nullable: true),
                    TestId = table.Column<Guid>(type: "uuid", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TechnicalManagerScopeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalManagerRoutingRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicalManagerRoutingRules_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicalManagerRoutingRules_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicalManagerRoutingRules_Matrices_MatrixId",
                        column: x => x.MatrixId,
                        principalTable: "Matrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicalManagerRoutingRules_SampleCategories_SampleCategor~",
                        column: x => x.SampleCategoryId,
                        principalTable: "SampleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicalManagerRoutingRules_TechnicalManagerScopes_Technic~",
                        column: x => x.TechnicalManagerScopeId,
                        principalTable: "TechnicalManagerScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicalManagerRoutingRules_TestPanels_TestPanelId",
                        column: x => x.TestPanelId,
                        principalTable: "TestPanels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicalManagerRoutingRules_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerRoutingRules_Code",
                table: "TechnicalManagerRoutingRules",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerRoutingRules_CustomerId",
                table: "TechnicalManagerRoutingRules",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerRoutingRules_DepartmentId",
                table: "TechnicalManagerRoutingRules",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerRoutingRules_MatrixId",
                table: "TechnicalManagerRoutingRules",
                column: "MatrixId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerRoutingRules_Priority_IsActive",
                table: "TechnicalManagerRoutingRules",
                columns: new[] { "Priority", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerRoutingRules_SampleCategoryId",
                table: "TechnicalManagerRoutingRules",
                column: "SampleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerRoutingRules_TechnicalManagerScopeId",
                table: "TechnicalManagerRoutingRules",
                column: "TechnicalManagerScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerRoutingRules_TestId",
                table: "TechnicalManagerRoutingRules",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerRoutingRules_TestPanelId",
                table: "TechnicalManagerRoutingRules",
                column: "TestPanelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicalManagerRoutingRules");
        }
    }
}
