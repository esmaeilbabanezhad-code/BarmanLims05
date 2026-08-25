using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTestLimitChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestLimitChangeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceLimitId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentLOD = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    RequestedLOD = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    CurrentLOQ = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    RequestedLOQ = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    CurrentMinValue = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    RequestedMinValue = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    CurrentMaxValue = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    RequestedMaxValue = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    CurrentWarningLow = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    RequestedWarningLow = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    CurrentWarningHigh = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    RequestedWarningHigh = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    Reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApprovedByEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ApprovalComment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestLimitChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestLimitChangeRequests_Employees_ApprovedByEmployeeId",
                        column: x => x.ApprovedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestLimitChangeRequests_Employees_RequestedByEmployeeId",
                        column: x => x.RequestedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestLimitChangeRequests_ReferenceLimits_ReferenceLimitId",
                        column: x => x.ReferenceLimitId,
                        principalTable: "ReferenceLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestLimitChangeRequests_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitChangeRequests_ApprovedByEmployeeId",
                table: "TestLimitChangeRequests",
                column: "ApprovedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitChangeRequests_ReferenceLimitId",
                table: "TestLimitChangeRequests",
                column: "ReferenceLimitId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitChangeRequests_RequestedByEmployeeId",
                table: "TestLimitChangeRequests",
                column: "RequestedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitChangeRequests_TestId_Status",
                table: "TestLimitChangeRequests",
                columns: new[] { "TestId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestLimitChangeRequests");
        }
    }
}
