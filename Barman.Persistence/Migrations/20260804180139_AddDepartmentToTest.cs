using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentToTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Tests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_DepartmentId",
                table: "Tests",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Departments_DepartmentId",
                table: "Tests",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Departments_DepartmentId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_DepartmentId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Tests");
        }
    }
}
