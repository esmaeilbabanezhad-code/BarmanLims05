using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleProductInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchLotNumber",
                table: "Samples",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerSampleName",
                table: "Samples",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Samples",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionDate",
                table: "Samples",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuotaNumber",
                table: "Samples",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipmentNumber",
                table: "Samples",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchLotNumber",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "CustomerSampleName",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "ProductionDate",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "QuotaNumber",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "ShipmentNumber",
                table: "Samples");
        }
    }
}
