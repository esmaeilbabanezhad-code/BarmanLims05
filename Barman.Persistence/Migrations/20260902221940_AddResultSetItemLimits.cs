using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddResultSetItemLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LOD",
                table: "TestResultSetItems",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LOQ",
                table: "TestResultSetItems",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxValue",
                table: "TestResultSetItems",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinValue",
                table: "TestResultSetItems",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LOD",
                table: "TestResultSetItems");

            migrationBuilder.DropColumn(
                name: "LOQ",
                table: "TestResultSetItems");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "TestResultSetItems");

            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "TestResultSetItems");
        }
    }
}
