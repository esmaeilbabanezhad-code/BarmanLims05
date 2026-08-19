using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentResponsibilityHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentResponsibilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponsibilityType = table.Column<int>(type: "integer", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentResponsibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentResponsibilities_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentResponsibilities_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicalManagerSectionHeads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnicalManagerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionHeadId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalManagerSectionHeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicalManagerSectionHeads_Employees_SectionHeadId",
                        column: x => x.SectionHeadId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicalManagerSectionHeads_Employees_TechnicalManagerId",
                        column: x => x.TechnicalManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentResponsibilities_DepartmentId",
                table: "DepartmentResponsibilities",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentResponsibilities_EmployeeId_DepartmentId_Responsi~",
                table: "DepartmentResponsibilities",
                columns: new[] { "EmployeeId", "DepartmentId", "ResponsibilityType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerSectionHeads_SectionHeadId",
                table: "TechnicalManagerSectionHeads",
                column: "SectionHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalManagerSectionHeads_TechnicalManagerId_SectionHead~",
                table: "TechnicalManagerSectionHeads",
                columns: new[] { "TechnicalManagerId", "SectionHeadId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentResponsibilities");

            migrationBuilder.DropTable(
                name: "TechnicalManagerSectionHeads");
        }
    }
}
