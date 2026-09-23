using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowStageToTestAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkflowStage",
                table: "TestAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // ==========================================
            // Migrate existing TestAssignments
            // ==========================================

            // Completed
            migrationBuilder.Sql("""
                UPDATE "TestAssignments"
                SET "WorkflowStage" = 60
                WHERE "IsApprovedByDirector" = TRUE;
                """);

            // Director Result Approval
            migrationBuilder.Sql("""
                UPDATE "TestAssignments"
                SET "WorkflowStage" = 50
                WHERE "IsApprovedBySection" = TRUE
                  AND "IsApprovedByTechManager" = TRUE
                  AND "IsApprovedByDirector" = FALSE
                  AND NULLIF(TRIM("Result"), '') IS NOT NULL;
                """);

            // Technical Manager Result Approval
            migrationBuilder.Sql("""
                UPDATE "TestAssignments"
                SET "WorkflowStage" = 40
                WHERE "IsApprovedBySection" = TRUE
                  AND "IsApprovedByTechManager" = FALSE
                  AND NULLIF(TRIM("Result"), '') IS NOT NULL;
                """);

            // Section Result Approval
            migrationBuilder.Sql("""
                UPDATE "TestAssignments"
                SET "WorkflowStage" = 30
                WHERE "IsApprovedBySection" = FALSE
                  AND "AnalystId" IS NOT NULL
                  AND NULLIF(TRIM("Result"), '') IS NOT NULL;
                """);

            // Analyst Work
            migrationBuilder.Sql("""
                UPDATE "TestAssignments"
                SET "WorkflowStage" = 20
                WHERE "IsApprovedBySection" = FALSE
                  AND "AnalystId" IS NOT NULL
                  AND NULLIF(TRIM("Result"), '') IS NULL;
                """);

            // Section Assignment
            migrationBuilder.Sql("""
                UPDATE "TestAssignments"
                SET "WorkflowStage" = 10
                WHERE "IsApprovedByTechManager" = TRUE
                  AND "AnalystId" IS NULL
                  AND "IsApprovedBySection" = FALSE;
                """);

            // Remaining records stay at TechnicalManagerAssignment (0).
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkflowStage",
                table: "TestAssignments");
        }
    }
}
