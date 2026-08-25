using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicalManagerApprovalToTestLimitChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TechnicalManagerApprovalComment",
                table: "TestLimitChangeRequests",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TechnicalManagerApprovedAt",
                table: "TestLimitChangeRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TechnicalManagerApprovedByEmployeeId",
                table: "TestLimitChangeRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TechnicalManagerStatus",
                table: "TestLimitChangeRequests",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_TestLimitChangeRequests_TechnicalManagerApprovedByEmployeeId",
                table: "TestLimitChangeRequests",
                column: "TechnicalManagerApprovedByEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestLimitChangeRequests_Employees_TechnicalManagerApprovedB~",
                table: "TestLimitChangeRequests",
                column: "TechnicalManagerApprovedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestLimitChangeRequests_Employees_TechnicalManagerApprovedB~",
                table: "TestLimitChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_TestLimitChangeRequests_TechnicalManagerApprovedByEmployeeId",
                table: "TestLimitChangeRequests");

            migrationBuilder.DropColumn(
                name: "TechnicalManagerApprovalComment",
                table: "TestLimitChangeRequests");

            migrationBuilder.DropColumn(
                name: "TechnicalManagerApprovedAt",
                table: "TestLimitChangeRequests");

            migrationBuilder.DropColumn(
                name: "TechnicalManagerApprovedByEmployeeId",
                table: "TestLimitChangeRequests");

            migrationBuilder.DropColumn(
                name: "TechnicalManagerStatus",
                table: "TestLimitChangeRequests");
        }
    }
}
