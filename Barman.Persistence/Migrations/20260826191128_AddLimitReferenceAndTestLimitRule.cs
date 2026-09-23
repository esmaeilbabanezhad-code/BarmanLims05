using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLimitReferenceAndTestLimitRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SelectedLimitRuleId",
                table: "TestAssignments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "ReferenceLimits",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SampleCategoryId",
                table: "ReferenceLimits",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LimitReference",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentNo = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitReference", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestLimitRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatrixId = table.Column<Guid>(type: "uuid", nullable: true),
                    SampleCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    LimitReferenceId = table.Column<Guid>(type: "uuid", nullable: false),
                    LimitType = table.Column<int>(type: "integer", nullable: false),
                    LowerValue = table.Column<decimal>(type: "numeric", nullable: true),
                    UpperValue = table.Column<decimal>(type: "numeric", nullable: true),
                    LowerInclusive = table.Column<bool>(type: "boolean", nullable: false),
                    UpperInclusive = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedValues = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestLimitRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestLimitRules_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestLimitRules_LimitReference_LimitReferenceId",
                        column: x => x.LimitReferenceId,
                        principalTable: "LimitReference",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestLimitRules_Matrices_MatrixId",
                        column: x => x.MatrixId,
                        principalTable: "Matrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestLimitRules_SampleCategories_SampleCategoryId",
                        column: x => x.SampleCategoryId,
                        principalTable: "SampleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestLimitRules_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestAssignments_SelectedLimitRuleId",
                table: "TestAssignments",
                column: "SelectedLimitRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceLimits_CustomerId",
                table: "ReferenceLimits",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceLimits_SampleCategoryId",
                table: "ReferenceLimits",
                column: "SampleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitRules_CustomerId",
                table: "TestLimitRules",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitRules_LimitReferenceId",
                table: "TestLimitRules",
                column: "LimitReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitRules_MatrixId",
                table: "TestLimitRules",
                column: "MatrixId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitRules_SampleCategoryId",
                table: "TestLimitRules",
                column: "SampleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitRules_TestId_CustomerId_MatrixId_SampleCategoryId_~",
                table: "TestLimitRules",
                columns: new[] { "TestId", "CustomerId", "MatrixId", "SampleCategoryId", "IsActive", "ValidFrom", "ValidTo", "Priority" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceLimits_Customers_CustomerId",
                table: "ReferenceLimits",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceLimits_SampleCategories_SampleCategoryId",
                table: "ReferenceLimits",
                column: "SampleCategoryId",
                principalTable: "SampleCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAssignments_TestLimitRules_SelectedLimitRuleId",
                table: "TestAssignments",
                column: "SelectedLimitRuleId",
                principalTable: "TestLimitRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceLimits_Customers_CustomerId",
                table: "ReferenceLimits");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceLimits_SampleCategories_SampleCategoryId",
                table: "ReferenceLimits");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAssignments_TestLimitRules_SelectedLimitRuleId",
                table: "TestAssignments");

            migrationBuilder.DropTable(
                name: "TestLimitRules");

            migrationBuilder.DropTable(
                name: "LimitReference");

            migrationBuilder.DropIndex(
                name: "IX_TestAssignments_SelectedLimitRuleId",
                table: "TestAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ReferenceLimits_CustomerId",
                table: "ReferenceLimits");

            migrationBuilder.DropIndex(
                name: "IX_ReferenceLimits_SampleCategoryId",
                table: "ReferenceLimits");

            migrationBuilder.DropColumn(
                name: "SelectedLimitRuleId",
                table: "TestAssignments");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ReferenceLimits");

            migrationBuilder.DropColumn(
                name: "SampleCategoryId",
                table: "ReferenceLimits");
        }
    }
}
