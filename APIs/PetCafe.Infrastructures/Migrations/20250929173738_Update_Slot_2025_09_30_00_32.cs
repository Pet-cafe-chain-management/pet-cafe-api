using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCafe.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Update_Slot_2025_09_30_00_32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "duration_minutes",
                table: "slots");

            migrationBuilder.DropColumn(
                name: "slot_datetime",
                table: "slots");

            migrationBuilder.AddColumn<string>(
                name: "applicable_days",
                table: "slots",
                type: "json",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "applicable_days",
                table: "slots");

            migrationBuilder.AddColumn<DateOnly>(
                name: "date",
                table: "slots",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "duration_minutes",
                table: "slots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "slot_datetime",
                table: "slots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
